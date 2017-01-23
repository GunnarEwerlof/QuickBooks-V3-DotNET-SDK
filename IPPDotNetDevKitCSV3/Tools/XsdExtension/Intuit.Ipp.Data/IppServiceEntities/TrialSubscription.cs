﻿////*********************************************************
// <copyright file="TrialSubscription.cs" company="Intuit">
//     Copyright (c) Intuit. All rights reserved.
// </copyright>
// <summary>This file contains class that encapsulates subscriber information as returend by API_GetAdminsForAllProducts.</summary>
////*********************************************************
namespace Intuit.Ipp.Data 
{
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Xml;

    /// <summary>
    /// Encapsulates Trial Subscription information as returend by API_IPPDevCustomerDetail.
    /// </summary>
    public class TrialSubscription 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrialSubscription"/> class.
        /// </summary>
        /// <param name="singleUserNode">The single user node.</param>
        public TrialSubscription(XmlNode singleUserNode)
        {
            XmlNode xmlNode = singleUserNode.SelectSingleNode("./startDate");
            if (xmlNode != null)
            {
                this.StartDate = xmlNode.InnerText;
            }

            xmlNode = singleUserNode.SelectSingleNode("./customerName");
            if (xmlNode != null)
            {
                this.CustomerName = xmlNode.InnerText;
            }

            xmlNode = singleUserNode.SelectSingleNode("./customerEmail");
            if (xmlNode != null)
            {
                this.CustomerEmail = xmlNode.InnerText;
            }

            xmlNode = singleUserNode.SelectSingleNode("./customerPhone");
            if (xmlNode != null)
            {
                this.CustomerPhone = xmlNode.InnerText;
            }

            xmlNode = singleUserNode.SelectSingleNode("./realm");
            if (xmlNode != null)
            {
                this.Realm = xmlNode.InnerText;
            }

            xmlNode = singleUserNode.SelectSingleNode("./applicationName");
            if (xmlNode != null)
            {
                this.ApplicationName = xmlNode.InnerText;
            }

            xmlNode = singleUserNode.SelectSingleNode("./planName");
            if (xmlNode != null)
            {
                this.PlanName = xmlNode.InnerText;
            }

            xmlNode = singleUserNode.SelectSingleNode("./isBeta");
            if (xmlNode != null)
            {
                this.IsBeta = xmlNode.InnerText;
            }

            xmlNode = singleUserNode.SelectSingleNode("./mbHours");
            if (xmlNode != null)
            {
                this.MbHours = xmlNode.InnerText;
            }

            xmlNode = singleUserNode.SelectSingleNode("./lastSyncTime");
            if (xmlNode != null)
            {
                this.LastSyncTime = xmlNode.InnerText;
            }
        }

        #region Properties

        /// <summary>
        /// Gets the start date.
        /// </summary>
        [Obfuscation(Exclude = true, Feature = "renaming", StripAfterObfuscation = true)]
        [Obfuscation(Exclude = false, Feature = "trigger", StripAfterObfuscation = true)]
        public string StartDate { get; private set; }

        /// <summary>
        /// Gets the name of the customer.
        /// </summary>
        /// <value>
        /// The name of the customer.
        /// </value>
        [Obfuscation(Exclude = true, Feature = "renaming", StripAfterObfuscation = true)]
        [Obfuscation(Exclude = false, Feature = "trigger", StripAfterObfuscation = true)]
        public string CustomerName { get; private set; }

        /// <summary>
        /// Gets the customer email.
        /// </summary>
        [Obfuscation(Exclude = true, Feature = "renaming", StripAfterObfuscation = true)]
        [Obfuscation(Exclude = false, Feature = "trigger", StripAfterObfuscation = true)]
        public string CustomerEmail { get; private set; }

        /// <summary>
        /// Gets the customer phone number.
        /// </summary>
        [Obfuscation(Exclude = true, Feature = "renaming", StripAfterObfuscation = true)]
        [Obfuscation(Exclude = false, Feature = "trigger", StripAfterObfuscation = true)]
        public string CustomerPhone { get; private set; }

        /// <summary>
        /// Gets the realm.
        /// </summary>
        [Obfuscation(Exclude = true, Feature = "renaming", StripAfterObfuscation = true)]
        [Obfuscation(Exclude = false, Feature = "trigger", StripAfterObfuscation = true)]
        public string Realm { get; private set; }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        [Obfuscation(Exclude = true, Feature = "renaming", StripAfterObfuscation = true)]
        [Obfuscation(Exclude = false, Feature = "trigger", StripAfterObfuscation = true)]
        public string ApplicationName { get; private set; }

        /// <summary>
        /// Gets the name of the plan.
        /// </summary>
        /// <value>
        /// The name of the plan.
        /// </value>
        [Obfuscation(Exclude = true, Feature = "renaming", StripAfterObfuscation = true)]
        [Obfuscation(Exclude = false, Feature = "trigger", StripAfterObfuscation = true)]
        public string PlanName { get; private set; }

        /// <summary>
        /// Gets the is beta.
        /// </summary>
        [Obfuscation(Exclude = true, Feature = "renaming", StripAfterObfuscation = true)]
        [Obfuscation(Exclude = false, Feature = "trigger", StripAfterObfuscation = true)]
        public string IsBeta { get; private set; }

        /// <summary>
        /// Gets the mb hours.
        /// </summary>
        [Obfuscation(Exclude = true, Feature = "renaming", StripAfterObfuscation = true)]
        [Obfuscation(Exclude = false, Feature = "trigger", StripAfterObfuscation = true)]
        public string MbHours { get; private set; }

        /// <summary>
        /// Gets the last sync time.
        /// </summary>
        [Obfuscation(Exclude = true, Feature = "renaming", StripAfterObfuscation = true)]
        [Obfuscation(Exclude = false, Feature = "trigger", StripAfterObfuscation = true)]
        public string LastSyncTime { get; private set; }
        #endregion

        /// <summary>
        /// Parses the trial subscription.
        /// </summary>
        /// <param name="node">The xmlnode.</param>
        /// <returns>
        /// Returns the trail subscriptions.
        /// </returns>
        public static Collection<TrialSubscription> ParseTrialSubscription(XmlNode node)
        {
            Collection<TrialSubscription> trialSubscriptionCollections = new Collection<TrialSubscription>();
            XmlNodeList admins = node.SelectNodes("./TrialSubscription");
            if (admins != null)
            {
                foreach (XmlNode admin in admins)
                {
                    trialSubscriptionCollections.Add(new TrialSubscription(admin));
                }
            }

            return trialSubscriptionCollections;
        }
    }
}
