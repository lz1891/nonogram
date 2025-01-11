using UnityEngine;
using Lean.Gui;

public class ToggleSoundButton : MonoBehaviour
{
    public LeanToggle leanToggle;

    private void Start()
    {
        if (SoundManager.instance != null && leanToggle != null)
        {
            leanToggle.Set(SoundManager.instance.IsSoundOn);
        }
    }

    public void UpdateToggleVisuals()
    {
        if (SoundManager.instance != null)
        {
            leanToggle.Set(SoundManager.instance.IsSoundOn);
        }
    }
}
