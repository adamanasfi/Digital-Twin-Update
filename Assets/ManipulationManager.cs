using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationManager : MonoBehaviour
{
    private AxisFlags translationCombinedAxes, rotationCombinedAxes;
    
    void Start()
    {

    }

    public void ToggleXAxis(bool isOn)
    {
        UpdateAxis(AxisFlags.XAxis, isOn);
    }

    public void ToggleYAxis(bool isOn)
    {
        UpdateAxis(AxisFlags.YAxis, isOn);
    }

    public void ToggleZAxis(bool isOn)
    {
        UpdateAxis(AxisFlags.ZAxis, isOn);
    }

    public void ToggleRotation(bool isOn)
    {
        rotationCombinedAxes |= AxisFlags.XAxis;
        rotationCombinedAxes |= AxisFlags.ZAxis;
        if (isOn)
        {
            rotationCombinedAxes |= AxisFlags.YAxis;
        }
        else
        {
            rotationCombinedAxes &= ~AxisFlags.YAxis;
        }
        foreach (Transform child in PrefabsManager.stodParent.transform)
        {
            RotationAxisConstraint rotationConstraint = child.GetComponent<RotationAxisConstraint>();
            rotationConstraint.ConstraintOnRotation = rotationCombinedAxes;
        }
    }

    private void UpdateAxis(AxisFlags axis, bool isOn)
    {
        if (isOn)
        {
            // Add the axis to the combined axes
            translationCombinedAxes |= axis;
        }
        else
        {
            // Remove the axis from the combined axes
            translationCombinedAxes &= ~axis;
        }

        foreach (Transform child in PrefabsManager.stodParent.transform)
        {
            MoveAxisConstraint moveConstraint = child.GetComponent<MoveAxisConstraint>(); 
            moveConstraint.ConstraintOnMovement = translationCombinedAxes;
            moveConstraint.UseLocalSpaceForConstraint = true;
        }
    }
}
