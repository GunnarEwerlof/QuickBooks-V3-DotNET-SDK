﻿////********************************************************************
// <copyright file="OAuthRequestValidator.cs" company="Intuit">
//     Copyright (c) Intuit. All rights reserved.
// </copyright>
// <summary>This file contains Request validate contract.</summary>
////********************************************************************

namespace Intuit.Ipp.Security
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Net;
    using System.Security.Cryptography;
    using DevDefined.OAuth.Consumer;
    using DevDefined.OAuth.Framework;
    using Intuit.Ipp.Exception;
    
    /// <summary>
    /// OAuth implementation for Request validate contract.
    /// </summary>
    public class OAuthRequestValidator : IRequestValidator
    {
        /// <summary>
        /// The Authorization Header constant.
        /// </summary>
        private const string AuthorizationHeader = "Authorization";

        /// <summary>
        /// The O auth signature method.
        /// </summary>
        private string oauthSignatureMethod;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthRequestValidator"/> class.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="accessTokenSecret">The access token secret.</param>
        /// <param name="consumerKey">The consumer key.</param> 
        /// <param name="consumerSecret">The consumer secret.</param>
        public OAuthRequestValidator(string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new InvalidTokenException("Access token cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(accessTokenSecret))
            {
                throw new InvalidTokenException("Access token secret cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(consumerKey))
            {
                throw new InvalidTokenException("Consumer key cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(consumerSecret))
            {
                throw new InvalidTokenException("Consumer key secret cannot be null or empty.");
            }

            this.AccessToken = accessToken;
            this.AccessTokenSecret = accessTokenSecret;
            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
            this.oauthSignatureMethod = SignatureMethod.HmacSha1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthRequestValidator"/> class.
        /// </summary>
        /// <param name="applicationToken">The application token.</param>
        public OAuthRequestValidator(string applicationToken)
        {
            this.ConsumerKey = applicationToken;
            this.oauthSignatureMethod = SignatureMethod.RsaSha1;
        }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the access token secret.
        /// </summary>
        /// <value>
        /// The access token secret.
        /// </value>
        public string AccessTokenSecret { get; set; }

        /// <summary>
        /// Gets or sets the consumer key.
        /// </summary>
        /// <value>
        /// The consumer key.
        /// </value>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Gets or sets the consumer secret.
        /// </summary>
        /// <value>
        /// The consumer secret.
        /// </value>
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// Gets or sets the additional parameters.
        /// </summary>
        /// <value>
        /// The additional parameters.
        /// </value>
        public NameValueCollection AdditionalParameters { get; set; }
        
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The asymmetric algorithm key.
        /// </value>
        public AsymmetricAlgorithm Key { get; set; }

        /// <summary>
        /// Authorizes the specified request.
        /// </summary>
        /// <param name="webRequest">The request.</param>
        /// <param name="requestBody">The requestBody if form encoded parameters.</param>
        public void Authorize(WebRequest webRequest, string requestBody)
        {
            IOAuthConsumerContext consumerContext = this.CreateConsumerContext(true);
            IOAuthSession oauthSession = CreateOAuthSessionWithConsumerContext(consumerContext);
            oauthSession.AccessToken = this.CreateAccessToken();
            string oauthHeader = this.GetOAuthHeaderForRequest(oauthSession, webRequest);
            webRequest.Headers.Add(AuthorizationHeader, oauthHeader);
        }
       
        /// <summary>
        /// Creates the OAuth session with consumer context.
        /// </summary>
        /// <param name="consumerContext">The consumer context.</param>
        /// <returns>return IOAuthSession.</returns>
        private static IOAuthSession CreateOAuthSessionWithConsumerContext(IOAuthConsumerContext consumerContext)
        {
            string requestTokenUri;
            requestTokenUri = ConfigurationManager.AppSettings["OAUTHREQUESTTOKENURI"];
            if (string.IsNullOrEmpty(requestTokenUri))
            {
                requestTokenUri = SecurityConstants.OAUTHREQUESTTOKENURI;
            }

            string authorizeRequestUrl;
            authorizeRequestUrl = ConfigurationManager.AppSettings["OAUTHAUTHORIZEREQUESTURL"];
            if (string.IsNullOrEmpty(authorizeRequestUrl))
            {
                authorizeRequestUrl = SecurityConstants.OAUTHAUTHORIZEREQUESTURL;
            }

            string accessTokenUrl;
            accessTokenUrl = ConfigurationManager.AppSettings["OAUTHACCESSTOKENURL"];
            if (string.IsNullOrEmpty(accessTokenUrl))
            {
                accessTokenUrl = SecurityConstants.OAUTHACCESSTOKENURL;
            }

            return new OAuthSession(consumerContext, requestTokenUri, authorizeRequestUrl, accessTokenUrl);
        }

        /// <summary>
        /// Gets the OAuth header for request.
        /// </summary>
        /// <param name="oauthSession">The OAuth session.</param>
        /// <param name="webRequest">The web request.</param>
        /// <returns>
        /// Returns the OAuth header
        /// </returns>
        private string GetOAuthHeaderForRequest(IOAuthSession oauthSession, WebRequest webRequest)
        {
            IConsumerRequest consumerRequest = oauthSession.Request();
            consumerRequest = ConsumerRequestExtensions.ForMethod(consumerRequest, webRequest.Method);
            consumerRequest = ConsumerRequestExtensions.ForUri(consumerRequest, webRequest.RequestUri);
            if (webRequest.Headers.Count > 0)
            {
                ConsumerRequestExtensions.AlterContext(consumerRequest, context => context.Headers = webRequest.Headers);
            }

            if (this.AdditionalParameters != null)
            {
                consumerRequest.Context.AuthorizationHeaderParameters.Add(this.AdditionalParameters);
            }

            consumerRequest = consumerRequest.SignWithToken();
            return consumerRequest.Context.GenerateOAuthParametersForHeader();
        }

        /// <summary>
        /// Creates the consumer context.
        /// </summary>
        /// <param name="isHeaderToBeAdded">if set to <c>true</c> header will be added to the request.</param>
        /// <returns>
        /// Returns IOAuthConsumerContext object.
        /// </returns>
        private IOAuthConsumerContext CreateConsumerContext(bool isHeaderToBeAdded)
        {
            IOAuthConsumerContext consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = this.ConsumerKey,
                ConsumerSecret = this.ConsumerSecret,
                SignatureMethod = this.oauthSignatureMethod,
            };

            if (this.Key != null)
            {
                consumerContext.Key = this.Key;
            }

            if (isHeaderToBeAdded)
            {
                consumerContext.UseHeaderForOAuthParameters = true;
            }

            return consumerContext;
        }

        /// <summary>
        /// Creates the access token.
        /// </summary>
        /// <returns>returns OAuth token.</returns>
        private IToken CreateAccessToken()
        {
            return new TokenBase
            {
                Token = this.AccessToken,
                ConsumerKey = this.ConsumerKey,
                TokenSecret = this.AccessTokenSecret
            };
        }
    }
}
