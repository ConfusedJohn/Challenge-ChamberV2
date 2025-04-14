using CommandSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Command that allows to place object on the map and remove them if we need to undo this operation.
/// </summary>
public class StructurePlacementCommand : ICommand
{
    PlacementManager placementManager;
    PlacementGridData placementData;
    ItemData itemData;
    SelectionResult selectionResult;    

    public StructurePlacementCommand(
        PlacementManager placementManager, 
        PlacementGridData placementData,
        ItemData itemData, 
        SelectionResult selectionResult)
    {
        this.placementManager = placementManager;
        this.selectionResult = selectionResult;
        this.placementData = placementData;
        this.itemData = itemData;
    }

    public bool CanExecute()
    {
        if (itemData != null && itemData.allowedNumber > 0)
        {
            return selectionResult.placementValidity;
        }
        else return false;
        
    }

    public void Execute()
    {
        itemData.allowedNumber -= 1;
        placementManager.PlaceStructureAt(selectionResult,placementData, this.itemData);

    }

    public void Undo()
    {
        itemData.allowedNumber += 1;
        placementManager.RemoveStructureAt(selectionResult, placementData);
    }
}
