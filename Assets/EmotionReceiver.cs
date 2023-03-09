using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
/*
 * Ce script permet de recevoir sur une socket les émotions détectés par le script Python
 */
public class EmotionReceiver : MonoBehaviour
{

    private TcpListener tcpListener;
    public int port;
    private byte[] receivebuffer;
    private TcpClient _client;
    private NetworkStream stream;
    // Start is called before the first frame update
    void Start()
    {
        //Ouverture de la socket
        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();
        Debug.Log("Serveur démarré");
        tcpListener.BeginAcceptTcpClient(new System.AsyncCallback(TCPCallback), null);
    }

    private void TCPCallback(IAsyncResult _result)
    {
        //Connection d'un client (le script python)
        _client = tcpListener.EndAcceptTcpClient(_result);
        Debug.Log("Client connecté");
        _client.ReceiveBufferSize = 4096;
        _client.SendBufferSize = 4096;
        stream = _client.GetStream();

        receivebuffer = new byte[4096];
        stream.BeginRead(receivebuffer, 0, 4096, ReceiveCallBack, null);

    }

    private void ReceiveCallBack(IAsyncResult _result)
    {
        //Réception des messages du client 
        try
        {
            int byteLength = stream.EndRead(_result);
            if (byteLength <= 0)
            {
                return;
            }
            byte[] data = new byte[byteLength];
            Array.Copy(receivebuffer, data,byteLength);
            //Pour l'instant on affiche l'émotion reçue dans la console de Unity.
            //On pourrait faire plus :)
            Debug.Log("Data reçu : "+ System.Text.Encoding.Default.GetString(receivebuffer));
            stream.BeginRead(receivebuffer, 0, 4096, ReceiveCallBack, null);
        }
        catch(Exception ex)
        {
            Debug.Log("Erreur serveur : " + ex.ToString());
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
