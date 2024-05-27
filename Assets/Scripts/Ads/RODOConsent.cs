using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Ump.Common;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class RODOConsent : MonoBehaviour
{
    public static RODOConsent Instance;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        // Set tag for under age of consent.
        // Here false means users are not under age of consent.
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
        };

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    private void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(consentError);
            return;
        }

        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            if (formError != null)
            {
                // Consent gathering failed.
                Debug.LogError(consentError);
                return;
            }

            // Consent has been gathered.
            if (ConsentInformation.CanRequestAds())
            {
                MobileAds.Initialize((InitializationStatus initstatus) =>
                {
                    AdsManager.Instance.LoadAd();
                });
            }
        });
    }
}
