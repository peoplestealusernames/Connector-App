using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
//TODO: Cleanup

public class Connection
{
    //TODO: error handling : Connection Ref, Connection Closed
    public TCPConnection TCP;//DO NOT WRITE TO THIS. THIS IS THE MAIN SOCKET
    public Event Events = new Event();

    private string remotePublicKey;
    private string PrivateKey;
    private string PublicKey;

    private bool RecHandShake = false;
    private bool Encrypted = false;

    public Connection(Host host)
    {
        Encryption.GenKeysSync(out this.PrivateKey, out this.PublicKey);

        this.TCP = new TCPConnection(host);
        this.TCP.Events.on("data", this.DataHandler);
        this.TCP.write("{" + '"' + "Pub" + '"' + ":" + '"' + this.PublicKey + '"' + "}");
    }

    private void DataHandler(object rawmsg)
    {
        string msg = rawmsg as string;
        if (Encrypted)
        {
            string Dec = Encryption.Decrypt(this.PrivateKey, msg);
            this.Events.emit("data", Dec);
            return;
        }//TODO: move this into the only function being called post "setup"

        try
        {
            PubKeyPasser Payload = new PubKeyPasser();
            JsonUtility.FromJsonOverwrite(msg, Payload);
            if (Payload.Pub != null)
            {
                this.remotePublicKey = Payload.Pub;

                this.TCP.write("PUBREC"); //TODO: send ack encypted
            }
        }
        catch (System.Exception err)
        {
            if (msg == "PUBREC")
            {
                this.RecHandShake = true;
            }
            else
            {
                Debug.Log(msg + "\n" + err.ToString());
            }
        }

        if (this.RecHandShake && this.remotePublicKey != null)
        {
            this.Encrypted = true;
            this.Events.emit("setup", true as object);
        }
    }

    public void write(string msg)
    {
        string Enc = Encryption.Encrypt(this.remotePublicKey, msg);
        this.TCP.write(Enc);
    }
    public void close()
    {
        this.TCP.close();
    }
}

class PubKeyPasser
{
    public string Pub;
}