using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;
using PlayFab.ClientModels;

public class AzureFunctionCaller : MonoBehaviour
{
    public string myKey = "inputValue";
    public string myValue = "Garrett";

    public string catalogName = "Lenses";
    public List<CatalogItem> catalog;
    public List<Lens> lenses = new List<Lens>();
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GetCatalog();
            //CallCSharpExecuteFunction();
    }

    public void DisplayCatalog()
    {
        foreach(Lens l in lenses)
        {
            string printTheThing = "~~~" + l.LensType + "~~~"
                + "\n" + l.Description
                + "\n Mercury Visibility: " + l.MercuryVisibility
                + "\n Venus Visibility: " + l.VenusVisibility
                + "\n Moon Visibility: " + l.MoonVisibility
                + "\n Mars Visibility: " + l.MarsVisibility
                + "\n Jupiter Visibility: " + l.JupiterVisibility
                + "\n Saturn Visibility: " + l.SaturnVisibility
                + "\n Uranus Visibility: " + l.UranusVisibility
                + "\n Neptune Visibility: " + l.NeptuneVisibility
                + "\n Pluto Visibility: " + l.PlutoVisibility;

            print(printTheThing);
        }
    }

    public void PrintCatalogItem(CatalogItem c)
    {
        string info = c.CustomData;
    }

    public void GetCatalog()
    {
        GetCatalogItemsRequest getCatalogRequest = new GetCatalogItemsRequest
        {
            CatalogVersion = catalogName
        };

        PlayFabClientAPI.GetCatalogItems(getCatalogRequest,
            result =>
            {
                catalog = result.Catalog;
            },
            error => Debug.Log(error.ErrorMessage)
        );

        Invoke("BreakApartCatalog", 3f);
    }

    public void BreakApartCatalog()
    {
        foreach(CatalogItem c in catalog)
        {
            Lens s = JsonUtility.FromJson<Lens>(c.CustomData);
            s.LensType = c.DisplayName;
            s.Description = c.Description;
            lenses.Add(s);
        }

        DisplayCatalog();
    }

    private void CallCSharpExecuteFunction()
    {
        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
                Type = PlayFabSettings.staticPlayer.EntityType, //Get this from when you logged in
            },
            FunctionName = "HelloWorld", //This should be the name of your Azure Function that you created.
            FunctionParameter = new Dictionary<string, object>() { { myKey, myValue } }, //This is the data that you would want to pass into your function.
            GeneratePlayStreamEvent = false //Set this to true if you would like this call to show up in PlayStream
        }, (ExecuteFunctionResult result) =>
        {
            if (result.FunctionResultTooLarge ?? false)
            {
                Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                return;
            }
            Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
            Debug.Log($"Result: {result.FunctionResult.ToString()}");
        }, (PlayFabError error) =>
        {
            Debug.Log($"Oops Something went wrong: {error.GenerateErrorReport()}");
        });
    }

}

public class Lens
{
    public string LensType;
    public string Description;
    public string MercuryVisibility;
    public string VenusVisibility;
    public string MoonVisibility;
    public string MarsVisibility;
    public string JupiterVisibility;
    public string SaturnVisibility;
    public string UranusVisibility;
    public string NeptuneVisibility;
    public string PlutoVisibility;
}
