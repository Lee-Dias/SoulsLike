using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class HardwareMouseController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float mouseSpeed = 1000f;
    private PlayerState playerState;
    void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
    }
    void Update()
    {

        if (playerState != null)
            if(!playerState.IsOnInventory && !playerState.IsInSettings && !playerState.IsOnBonfire) return;
        
        

        if (Gamepad.current == null || Mouse.current == null) return;

        // --- MOVIMENTO DO STICK ---
        Vector2 stickInput = Gamepad.current.leftStick.ReadValue();

        if (stickInput != Vector2.zero)
        {
            Vector2 currentPosition = Mouse.current.position.ReadValue();
            Vector2 newPosition = currentPosition + (stickInput * mouseSpeed * Time.unscaledDeltaTime);

            newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);

            Mouse.current.WarpCursorPosition(newPosition);
        }

        // --- CLIQUES DO COMANDO (Forma segura com MouseState) ---
        
        // Se o X (South) ou o Quadrado (West) foram pressionados ou soltos neste frame
        if (Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.buttonSouth.wasReleasedThisFrame ||
            Gamepad.current.buttonWest.wasPressedThisFrame || Gamepad.current.buttonWest.wasReleasedThisFrame)
        {
            // Criamos uma estrutura de estado baseada no rato real atual
            MouseState mouseState = new MouseState();
            
            // Mantém a posição atual do rato para não o teleportar sem querer
            mouseState.position = Mouse.current.position.ReadValue();

            // Define o clique esquerdo (X / Button South)
            if (Gamepad.current.buttonSouth.isPressed)
            {
                mouseState.WithButton(MouseButton.Left, true);
            }

            // Define o clique direito (Quadrado / Button West)
            if (Gamepad.current.buttonWest.isPressed)
            {
                mouseState.WithButton(MouseButton.Right, true);
            }

            // Envia o estado completo e corrigido para o Input System
            InputState.Change(Mouse.current, mouseState);
        }
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            // Isto obriga a UI a recalcular imediatamente quem está debaixo do rato
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }
}