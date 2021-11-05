using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RGBType : MonoBehaviour
{
    public InputField Red, Green, Blue;
    public Button White, Off, ColorWheel, Ref, SleepB;
    public GameObject Disconnected;

    public HttpsReq HttpsHandle; //TODO: better implimentation
    public string URI = ""; //TODO: move to config game object for setting
    public Connection Ser;
    public HoldButton ColorWheelHold;

    Color RGB;
    bool Sleep = false;
    bool SerConnected = false;//TODO: Update and check

    void FixedUpdate()
    {
        Disconnected.SetActive(!SerConnected);
    }

    void OnApplicationPause(bool focusStatus)
    {
        FixedUpdate();
        if (focusStatus)
        {
            Debug.Log("Con");
            Connect();
        }
        else
        {
            SerConnected = false;
            Debug.Log("Dis");
            Disconnect();
        }
    }

    void Start()
    {
        /*DataSaver URIFile = new DataSaver("C:/Pass/Mainframe/URI.txt");
        //TODO: Save file location in config game object
        if (URI != "")
        {
            URIFile.Save(URI);
            Debug.Log("Saved URI to file");
        }
        else
        {
            string Stri = URIFile.Load();
            if (Stri == "There is no save data!")
            {
                Debug.LogError("No save data set: URI");
            }
            URI = URIFile.Load();
        }*/
        Connect();

        Red.onEndEdit.AddListener(delegate { typed(Red, 0, "r"); });
        Blue.onEndEdit.AddListener(delegate { typed(Blue, 1, "b"); });
        Green.onEndEdit.AddListener(delegate { typed(Green, 2, "g"); });

        White.onClick.AddListener(delegate { SetColor(255, 255, 255, 255); });
        Off.onClick.AddListener(delegate { SetColor(0, 0, 0, 255); });

        Ref.onClick.AddListener(delegate { RefreshValues(); });
        SleepB.onClick.AddListener(delegate { ToggleSleep(); });

        ColorWheel.onClick.AddListener(delegate { GrabColor(); });
    }

    void Connect()
    {
        HttpsHandle.GetRequest(URI, ConnectRes);
    }

    void ConnectRes(string Res)
    {
        Host URI = new Host(Res);
        Debug.Log(URI);

        if (Ser != null)
            Ser.close();

        Ser = new Connection(URI);
        Ser.Events.on("setup", Connected);
        Ser.Events.on("data", Data);
    }

    void Disconnect()
    {
        Ser.close();
    }

    void Data(object rawmsg)
    {
        string msg = rawmsg as string;
        Debug.Log(msg);

        Request Req = new Request(msg);
        Debug.Log(Req.data);
        //TODO: swap to a DataBase event system like in node
        //if (Req.path == "SleepScreen")
    }

    void Connected(object N)
    {
        Debug.Log("Setup");
        SerConnected = true;
        RefreshValues();
        //TODO: on screen display
    }

    void ToggleSleep()
    {
        string Stri = "false";
        //TODO: get sleep and apply it to button
        if (!Sleep)
        {
            Stri = "true";
            Sleep = true;
            ButtonColor(SleepB, new Color(0, 0, 0, 255));
        }
        else
        {
            Stri = "false";
            Sleep = false;
            ButtonColor(SleepB, new Color(255, 255, 255, 255));
        }

        Request Push = new Request();
        Push.method = "PUT";
        Push.path = "SleepScreen";
        Ser.write(Push.Save(Stri));
    }

    void ButtonColor(Button But, Color C)
    {
        Image S = But.GetComponent<Image>();
        S.color = C;
    }

    /*void TexToFile(Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();
        Object.Destroy(tex);

        // For testing purposes, also write to a file in the project folder
        File.WriteAllBytes(Application.dataPath + "/SavedScreen.png", bytes);
    }*/
    void GrabColor()
    {
        //StartCoroutine(GrabColorCo());

        /*ColorWheelHold.Held = true;
        while (ColorWheelHold.Held)
        {//TODO: ColorwheelHold*/
        Vector2 BtnPos = ColorWheel.transform.position;

        Vector2 MPos = Input.mousePosition;
        Debug.Log(MPos);
        Vector2 LPos = MPos - BtnPos;

        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
        Color C = tex.GetPixel((int)MPos.x, (int)MPos.y);
        Object.Destroy(tex);

        for (int i = 0; i < 4; i++)
            RGB[i] = (int)(C[i] * 255);

        Debug.Log(C);

        UpdateDB();
        UpdateValues();
        /*    yield return new WaitForSeconds(1);
        }*/
    }
    void SetColor(int r, int g, int b, int a)
    {
        RGB = new Color(r, g, b, a);
        UpdateDB();
        UpdateValues();
    }
    void typed(InputField In, int Type, string CN)
    {
        int NV = int.Parse(In.text);
        if (NV != RGB[Type])
        {
            RGB[Type] = NV;
        }
        UpdateDB();
    }
    void UpdateDB()
    {
        string ReqStri = "{" +
        '"' + "r" + '"' + ":" + RGB.r.ToString() + "," +
        '"' + "g" + '"' + ":" + RGB.g.ToString() + "," +
        '"' + "b" + '"' + ":" + RGB.b.ToString() +
        "}";
        Request Push = new Request();
        Push.method = "PUT";
        Push.path = "RGB";
        Ser.write(Push.Save(ReqStri));
    }
    void RefreshValues(object In)
    { RefreshValues(); }
    void RefreshValues()
    {
        //TODO: Setup
    }
    void UpdateValues()
    {
        Red.text = RGB.r.ToString();
        Green.text = RGB.g.ToString();
        Blue.text = RGB.b.ToString();
    }
}
