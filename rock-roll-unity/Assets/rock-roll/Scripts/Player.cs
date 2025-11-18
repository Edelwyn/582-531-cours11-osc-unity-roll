using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class Player : MonoBehaviour
{
    public float torqueForce = 1f;
    public float jumpForce = 5f;

    public float checkRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;

    public extOSC.OSCReceiver oscReceiver;

    public static float Proportion(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin), outputMin, outputMax);
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        oscReceiver.Bind("/rota", TraiterOscRotation);
        oscReceiver.Bind("/bouton", TraiterOscBouton);
    }

    void TraiterOscRotation(OSCMessage message)
    {
        // Si le message n'a pas d'argument ou l'argument n'est pas un Int on l'ignore
        if (message.Values.Count == 0)
        {
            Debug.Log("No value in OSC message");
            return;
        }

        if (message.Values[0].Type != OSCValueType.Int)
        {
            Debug.Log("Value in message is not an Int");
            return;
        }

        // Récupérer la valeur de l’angle depuis le message OSC
        int value = message.Values[0].IntValue;
        float input = value / 1000f;
        // EXEMPLE : utiliser la valeur pour appliquer une rotation
        // Adapter proportionnellement la valeur reçue
        float mouvement = Proportion(value, -1000, 1000, -1, 1);
        // Roll left/right
        if (mouvement > 1)
        {
            rb.AddTorque(-input * torqueForce); // clockwise

        }
        if (mouvement < 0)
        {
            rb.AddTorque(input * torqueForce); // counter-clockwise
        }
    }
    void TraiterOscBouton(OSCMessage message)
    {
        // Si le message n'a pas d'argument ou l'argument n'est pas un Int on l'ignore
        if (message.Values.Count == 0)
        {
            Debug.Log("No value in OSC message");
            return;
        }

        if (message.Values[0].Type != OSCValueType.Int)
        {
            Debug.Log("Value in message is not an Int");
            return;
        }

        // Récupérer la valeur de l’angle depuis le message OSC
        int value = message.Values[0].IntValue;
        if (value == 0)
        {
            if (IsGrounded())
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                Debug.Log("pressed!");
            }
        }
    }

    void FixedUpdate()
    {

        // Roll left/right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.AddTorque(-torqueForce); // clockwise
           
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddTorque(torqueForce); // counter-clockwise
        }

       
    }

    void Update()
    {
         // Jump
         // GetKeyDown() does not work in FixedUpdate()
        if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            if (IsGrounded())
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                Debug.Log("pressed!");
            }
        }
    }

    public bool IsGrounded()
    {
        float extraHeight = 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            GetComponent<CircleCollider2D>().radius + extraHeight,
            groundLayer
        );
        return hit.collider != null;
    }
}
