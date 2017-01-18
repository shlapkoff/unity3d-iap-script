using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace IAP
{
    public class InAppManager : MonoBehaviour, IStoreListener
    {
        private static IStoreController m_StoreController;
        private static IExtensionProvider m_StoreExtensionProvider;

        public const string pMoney80 = "money_80";
        public const string pNoAds = "no_ads";

        public const string pMoney80AppStore = "app_money_80";
        public const string pNoAdsAppStore = "app_no_ads";

        public const string pMoney80GooglePlay = "gp_money_80";
        public const string pNoAdsGooglePlay = "gp_no_ads";

        void Start()
        {
            if (m_StoreController == null)
            {
                InitializePurchasing();
            }
        }

        public void InitializePurchasing()
        {
            if (IsInitialized())
            {
                return;
            }

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(pMoney80, ProductType.Consumable, new IDs() { { pMoney80AppStore, AppleAppStore.Name }, { pMoney80GooglePlay, GooglePlay.Name } });
            builder.AddProduct(pNoAds, ProductType.NonConsumable, new IDs() { { pNoAdsAppStore, AppleAppStore.Name }, { pNoAdsGooglePlay, GooglePlay.Name } });

            UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialized()
        {
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        public void BuyProductID(string productId)
        {
            try
            {
                if (IsInitialized())
                {
                    Product product = m_StoreController.products.WithID(productId);

                    if (product != null && product.availableToPurchase)
                    {
                        Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                        m_StoreController.InitiatePurchase(product);
                    }
                    else
                    {
                        Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    }
                }
                else
                {
                    Debug.Log("BuyProductID FAIL. Not initialized.");
                }
            }
            catch (Exception e)
            {
                Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
            }
        }

        public void RestorePurchases()
        {
            if (!IsInitialized())
            {
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                Debug.Log("RestorePurchases started ...");

                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                apple.RestoreTransactions((result) =>
                    {
                        Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                    });
            }
            else
            {
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("OnInitialized: Completed!");

            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            if (String.Equals(args.purchasedProduct.definition.id, pMoney80, StringComparison.Ordinal))
            {
                //Action for money
            }
            else if (String.Equals(args.purchasedProduct.definition.id, pNoAds, StringComparison.Ordinal))
            {
                //Action for no ads
            }

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }
    }
}