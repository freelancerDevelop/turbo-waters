using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Purchasing;
using GameAnalyticsSDK;

[Serializable]
public class ShopProductReceipt
{
    public string Store;
    public string TransactionID;
    public string Payload;
}

public class ShopManager : Singleton<ShopManager>, IStoreListener
{
    private IStoreController iapController;
    private IExtensionProvider iapExtensions;
    private IAppleExtensions iapAppleExtension;

    public void Start()
    {
        var iapBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        iapBuilder.AddProduct("noads.permanent", ProductType.NonConsumable, new IDs {
            { "noads.permanent", AppleAppStore.Name },
            { "noads.permanent", GooglePlay.Name }
        });

        UnityPurchasing.Initialize(this, iapBuilder);
    }

    public bool IsIapOwned(string productId)
    {
        return StorageManager.Instance.GetBool(string.Format(StorageKeys.OwnedIAP, productId), false);
    }

    public void MarkIapOwned(string productId)
    {
        StorageManager.Instance.SetBool(string.Format(StorageKeys.OwnedIAP, productId), true);
    }

    public void PurchaseIapProduct(string productId)
    {
        #if UNITY_ANDROID || UNITY_IOS
            if (this.iapController == null) {
                Logger.Error("No IAP controller available...");
                return;
            }

            this.iapController.InitiatePurchase(productId);
        #else
            Logger.Error("Unsupported platform for IAP...");
        #endif
    }

    public void RestoreIapPurchases()
    {
        this.iapAppleExtension.RestoreTransactions(delegate (bool wasRestored) {
            if (wasRestored) {
            } else {
            }
        });
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Logger.Message("Unity IAP store ready to process...");

        this.iapController = controller;
        this.iapExtensions = extensions;
        this.iapAppleExtension = this.iapExtensions.GetExtension<IAppleExtensions>();

        this.iapAppleExtension.RegisterPurchaseDeferredListener(this.OnDeferredPurchase);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Logger.ErrorFormat("Failed to initialize IAP store: {0}", error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        string productId = e.purchasedProduct.definition.id;
        string receipt = e.purchasedProduct.receipt;

        #if UNITY_IOS
            receipt = Formatter.Instance.ToJson(new ShopProductReceipt {
                Store = "AppleAppStore",
                TransactionID = e.purchasedProduct.transactionID,
                Payload = this.iapAppleExtension.GetTransactionReceiptForProduct(e.purchasedProduct)
            });
        #endif

        this.MarkIapOwned(productId);

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason error)
    {
        Logger.ErrorFormat("Failed to purchase IAP product: {0}, {1}", product, error);
    }

    private void OnDeferredPurchase(Product product)
    {
        // Pending ask to buy
    }
}
