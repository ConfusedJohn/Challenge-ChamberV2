using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// High Level class that connects the Input, Undo system and placement system
/// </summary>
public class PlacementManager : MonoBehaviour
{
    [SerializeField]
    private InputManager input;

    [SerializeField]
    private GridManager gridManager;

    [SerializeField]
    private ItemDataBaseSO structuresData;

    [SerializeField]
    private PlacementPreview placementPrevew;

    [SerializeField]
    private GameObject floorPrefab, wallPrefab;

    [SerializeField]
    private StructurePlacer structurePlacer;

    CommandManager commandManager = new CommandManager();

    GridData gridData;

    ItemData itemData;

    [SerializeField]
    private GameObject removeStateFrame;

    BuildingState buildingState;

    [SerializeField]
    private GameObject destoryPreview;

    public UnityEvent OnExitPlacementMode, OnPlaceConstructionObject, OnPlaceFurnitureObject, OnRemoveObject, OnUndo, OnRotate, OnExitMovement, OnMovementStateEntered;
    public UnityEvent<bool> OnToggleUndo;

    private void Start()
    {
        gridData = new GridData(gridManager.GridSize);
        gridManager.ToggleGrid(false);

        input.OnToggleDelete += HandleDeleteAction;
        input.OnCancle += () => OnExitPlacementMode?.Invoke();

        switch (SceneManager.GetActiveScene().name)
        {
            case "Level1":
                PlaceStrucuterAtMap(23, new Vector3(0.0f, 0.02f, -4.0f), new Vector3Int(0, 0, -8), new Quaternion(), new Quaternion());
                PlaceStrucuterAtMap(24, new Vector3(0.0f, 0.02f, 0f), new Vector3Int(0, 0, 0), new Quaternion(), new Quaternion());
                PlaceStrucuterAtMap(25, new Vector3(2.5f, 0.02f, -3.8f), new Vector3Int(5, 0, -7), new Quaternion(), new Quaternion());
                PlaceStrucuterAtMap(25, new Vector3(4f, 0.02f, -2.8f), new Vector3Int(8, 0, -5), new Quaternion(0.00000f,0.70711f, 0.00000f, -0.70711f), new Quaternion());
                PlaceStrucuterAtMap(26, new Vector3(0.5f, 0.02f, -1.5f), new Vector3Int(1, 0, -3), new Quaternion(), new Quaternion());
                PlaceStrucuterAtMap(26, new Vector3(3f, 0.02f, 0.5f), new Vector3Int(6, 0, 1), new Quaternion(0.00000f, 1.00000f, 0.00000f, 0.00000f), new Quaternion());
                PlaceStrucuterAtMap(23, new Vector3(-3.0f, 0.02f, 3.0f), new Vector3Int(-6, 0, 6), new Quaternion(), new Quaternion());
                PlaceStrucuterAtMap(27, new Vector3(3.0f, 0.02f, 3f), new Vector3Int(6, 0, 6), new Quaternion(0.00000f, 0.70711f, 0.00000f, -0.70711f), new Quaternion());
                PlaceStrucuterAtMap(28, new Vector3(-2f, 0.02f, 1f), new Vector3Int(-4, 0, 2), new Quaternion(0.00000f, 0.70711f, 0.00000f, 0.70711f), new Quaternion());
                PlaceStrucuterAtMap(34, new Vector3(-3.0f, 0.02f, -4.0f), new Vector3Int(-6, 0, -8), new Quaternion(), new Quaternion());
                break;
            case "Level2":
                PlaceStrucuterAtMap(34, new Vector3(0.0f, 0.02f, 0f), new Vector3Int(0, 0, 0), new Quaternion(), new Quaternion());
                PlaceStrucuterAtMap(29, new Vector3(0.0f, 0.02f, -3.8f), new Vector3Int(2, 0, -8), new Quaternion(0.00000f, 1.00000f, 0.00000f, 0.00000f), new Quaternion());
                PlaceStrucuterAtMap(30, new Vector3(-3.0f, 0.02f, -3f), new Vector3Int(-6, 0, -6), new Quaternion(), new Quaternion());
                PlaceStrucuterAtMap(30, new Vector3(3.0f, 0.02f, 3f), new Vector3Int(6, 0, 6), new Quaternion(0.00000f, 0.70711f, 0.00000f, 0.70711f), new Quaternion());
                PlaceStrucuterAtMap(33, new Vector3(-3.0f, 0.02f, 1.5f), new Vector3Int(-6, 0, 3), new Quaternion(), new Quaternion());
                PlaceStrucuterAtMap(31, new Vector3(4f, 0.02f, 0f), new Vector3Int(8, 0, 0), new Quaternion(), new Quaternion());
                PlaceStrucuterAtMap(31, new Vector3(-1.0f, 0.02f, -5f), new Vector3Int(-2, 0, -10), new Quaternion(0.00000f, 0.70711f, 0.00000f, 0.70711f), new Quaternion());
                PlaceStrucuterAtMap(32, new Vector3(2.0f, 0.02f, -2f), new Vector3Int(4, 0, -4), new Quaternion(), new Quaternion());
                PlaceStrucuterAtMap(32, new Vector3(-5f, 0.02f, -3f), new Vector3Int(-10, 0, -6), new Quaternion(0.00000f, 0.70711f, 0.00000f, 0.70711f), new Quaternion());
                break;
        }
        
    }

    /// <summary>
    /// Creates a placement State connects it to the Input system
    /// </summary>
    /// <param name="id"></param>
    public void StartPlacingObject(int id)
    {
        CancelMovement();
        if (buildingState != null)
            CancelState();

        itemData = structuresData.GetItemWithID(id);
        if (itemData == null)
        {
            Debug.LogError($"No idem with id{id}");
        }
        buildingState = new PlacingObjectsState(gridManager, gridData, itemData);
        buildingState.OnFinished += TryPlacingObjects;
        buildingState.OnSelectionChanged += MovePreview;
        gridManager.ToggleGrid(true);
        placementPrevew.StartShowingPreview(itemData.previewObject);
        ConnectInputToBuildingState();
    }

    /// <summary>
    /// Connects input to the current Building State
    /// </summary>
    private void ConnectInputToBuildingState()
    {
        input.OnMousePressed += HandleSelectionStarted;
        input.OnMouseReleased += HandleSelectionFinished;
        input.OnCancle += CancelState;
        input.OnUndo += TryUndoLastPlacement;
        input.OnRotate += HandleRotation;
    }

    private void HandleRotation(int modifier)
    {
        buildingState.HandleRotation(modifier);
        OnRotate?.Invoke();
    }



    /// <summary>
    /// Created anf invokes a Command responsible for placing objects and undoing placement
    /// </summary>
    /// <param name="selectionResult"></param>
    private void TryPlacingObjects(SelectionResult selectionResult)
    {

        if (itemData.objectPlacementType == PlacementType.InWalls)
        {
            int previousItemIndex = gridData.WallPlacementData.GetStructureIDForEdgeObject(
                buildingState.SelectionData.GetSelectedGridPositions()[0],
                Mathf.RoundToInt(buildingState.SelectionData.GetSelectedPositionsGridRotation()[0].eulerAngles.y));

            commandManager.Invoke(
                new StructureSwapCommand(
                    this,
                    buildingState.CurrentPlacementData,
                    gridData.WallPlacementData,
                    gridManager,
                    itemData,
                    selectionResult,
                    structuresData.GetItemWithID(previousItemIndex)
                    )
            );
            if (selectionResult.placementValidity)
            {
                OnPlaceConstructionObject?.Invoke();
            }
        }
        else
        {
            commandManager.Invoke(
            new StructurePlacementCommand(
                this,
                buildingState.CurrentPlacementData,
                itemData,
                selectionResult
                )
        );
            if (selectionResult.placementValidity)
            {
                if (itemData.objectPlacementType == PlacementType.Floor || itemData.objectPlacementType == PlacementType.Wall)
                    OnPlaceConstructionObject?.Invoke();
                else
                    OnPlaceFurnitureObject?.Invoke();
            }

        }


    }

    /// <summary>
    /// Creates a command responsible for removing objects
    /// </summary>
    /// <param name="selectionResult"></param>
    public void TryRemovingObject(SelectionResult selectionResult)
    {
       

        commandManager.Invoke(
                new StructureRemoveCommand(
                    gridManager,
                    this,
                    buildingState.CurrentPlacementData,
                    itemData,
                    selectionResult
                    )
            );
        OnRemoveObject?.Invoke();
    }

    /// <summary>
    /// Moves preview of the placed objects
    /// </summary>
    /// <param name="selectionResult"></param>
    private void MovePreview(SelectionResult selectionResult)
    {
        placementPrevew.MovePreview(selectionResult.selectedPreviewPositions, selectionResult.selectedPositionsObjectRotation);

        //In remove mode always show the preview as red
        if (buildingState is RemovingState)
            placementPrevew.ShowPlacementFeedback(false);
        else
            placementPrevew.ShowPlacementFeedback(selectionResult.placementValidity);
    }

    /// <summary>
    /// Calls Undo operation on the available Command
    /// </summary>
    public void TryUndoLastPlacement()
    {
        if (commandManager.Undo() == false)
            Debug.Log("Can't undo placement");
        else
            OnUndo?.Invoke();
        if (commandManager.GetCommandsCount() <= 0)
            OnToggleUndo?.Invoke(false);
    }

    /// <summary>
    /// Stops building state and disconnects it from the input system
    /// </summary>
    public void CancelState()
    {
        if (buildingState == null)
            return;
        //buildingState.ExitState();
        buildingState = null;
        OnToggleUndo?.Invoke(false);
        input.OnMousePressed -= HandleSelectionStarted;
        input.OnMouseReleased -= HandleSelectionFinished;
        input.OnCancle -= CancelState;
        input.OnUndo -= TryUndoLastPlacement;
        input.OnRotate -= HandleRotation;
        commandManager.ClearCommandsList();
        placementPrevew.StopShowingPreview();
        gridManager.ToggleGrid(false);
    }

    /// <summary>
    /// Triggers the Removing State based on the current state that we are in.
    /// Ex if we are placing walls it will trigger RemoveWalls functionality
    /// </summary>
    /// <param name="val"></param>
    private void HandleDeleteAction(bool val)
    {
        if (buildingState == null)
            return;
        removeStateFrame.SetActive(val);
        //Save rotation between Removing and Placement states
        Quaternion previousRotation = buildingState.SelectionData.Rotation;
        CancelState();


       

        if (val)
        {
            buildingState = new RemovingState(gridManager, gridData, itemData);
            buildingState.SelectionData.Rotation = previousRotation;
            buildingState.OnFinished += TryRemovingObject;
            buildingState.OnSelectionChanged += MovePreview;
            gridManager.ToggleGrid(true);
            if (itemData.objectPlacementType.IsObjectPlacement())
                placementPrevew.StartShowingPreview(destoryPreview, true);
            else
                placementPrevew.StartShowingPreview(itemData.previewObject);
            ConnectInputToBuildingState();
        }
        else
        {
            //rester previous strategy
            StartPlacingObject(itemData.ID);
            buildingState.SelectionData.Rotation = previousRotation;
        }
    }

    /// <summary>
    /// Allows to place objects on the map as well as to save them in our Grid Data object
    /// </summary>
    /// <param name="selectionResult"></param>
    /// <param name="placementData"></param>
    /// <param name="itemData"></param>
    /// <param name="isInWallObject"></param>
    public void PlaceStructureAt(SelectionResult selectionResult, PlacementGridData placementData, ItemData itemData)
    {
        for (int i = 0; i < selectionResult.selectedGridPositions.Count; i++)
        {
            if (itemData.objectPlacementType.IsEdgePlacement())
            {
                int objectIndex = structurePlacer.PlaceStructure(itemData.prefab, selectionResult.selectedPositions[i], selectionResult.selectedPositionsObjectRotation[i], 0);
                placementData.AddEdgeObject(objectIndex, itemData.ID, selectionResult.selectedGridPositions[i], itemData.size, Mathf.RoundToInt(selectionResult.selectedPositionGridCheckRotation[i].eulerAngles.y));
            }
            else
            {
                int objectIndex = structurePlacer.PlaceStructure(itemData.prefab, selectionResult.selectedPositions[i], selectionResult.selectedPositionsObjectRotation[i], 0);
                placementData.AddCellObject(objectIndex, itemData.ID, selectionResult.selectedGridPositions[i], itemData.size, Mathf.RoundToInt(selectionResult.selectedPositionGridCheckRotation[i].eulerAngles.y));
                Debug.Log("rest"+ selectionResult.selectedPositions[i]+" "+ selectionResult.selectedPositionsObjectRotation[i]);
                //Debug.Log("rrest" + objectIndex + " " + itemData.ID + " " + selectionResult.selectedGridPositions[i] + " " + itemData.size + " " + Mathf.RoundToInt(selectionResult.selectedPositionGridCheckRotation[i].eulerAngles.y));

                //Debug.Log("1 "+selectionResult.selectedPositionsObjectRotation[i]);
                //Debug.Log("2 " + Mathf.RoundToInt(selectionResult.selectedPositionGridCheckRotation[i].eulerAngles.y));
            }

        }
        buildingState.RefreshSelection();
        OnToggleUndo?.Invoke(true);
    }

    private void PlaceStrucuterAtMap(int id, Vector3 selectedPositions, Vector3Int selectedGridPositions, Quaternion selectedPositionsObjectRotation, Quaternion selectedPositionGridCheckRotation)
    {
        if (buildingState != null)
            CancelState();

        itemData = structuresData.GetItemWithID(id);
        if (itemData == null)
        {
            Debug.LogError($"No idem with id{id}");
        }
        buildingState = new PlacingObjectsState(gridManager, gridData, itemData);

        int objectIndex = structurePlacer.PlaceStructure(structuresData.GetItemWithID(id).prefab, selectedPositions, selectedPositionsObjectRotation, 0);
        buildingState.CurrentPlacementData.AddCellObject(objectIndex, id, selectedGridPositions, structuresData.GetItemWithID(id).size, Mathf.RoundToInt(selectedPositionGridCheckRotation.eulerAngles.y));

        CancelState();
    }

    /// <summary>
    /// Removes a structre at a specific positions
    /// </summary>
    /// <param name="selectionResult"></param>
    /// <param name="placementData"></param>
    public void RemoveStructureAt(SelectionResult selectionResult, PlacementGridData placementData)
    {
        for (int i = 0; i < selectionResult.selectedGridPositions.Count; i++)
        {
            if (selectionResult.isEdgeStructure)
            {
                int index = placementData.GetIndexForEdgeObject(selectionResult.selectedGridPositions[i], Mathf.RoundToInt(selectionResult.selectedPositionGridCheckRotation[i].eulerAngles.y));
                if (index > -1)
                {
                    placementData.RemoveEdgeObject(selectionResult.selectedGridPositions[i], selectionResult.size, Mathf.RoundToInt(selectionResult.selectedPositionGridCheckRotation[i].eulerAngles.y));
                    structurePlacer.RemoveObjectAt(index);
                }
            }
            else
            {
                int index = placementData.GetIndexForCellObject(selectionResult.selectedGridPositions[i]);
                if (index > -1)
                {
                    
                    //Vector3 selectedPosition = input.GetSelectedMapPosition();
                    previousPosition = gridManager.GetCellPosition(selectionResult.selectedPositions[i], PlacementType.FreePlacedObject);

                    //Debug.Log("test " + structuresData.GetItemWithID(gridData.ObjectPlacementData.GetStructureIDForCellObject(previousPosition.Value)).ID);
                    //
                        if (structuresData.GetItemWithID(gridData.ObjectPlacementData.GetStructureIDForCellObject(previousPosition.Value)).ID > 22)
                        {
                            return;
                        }
                   
                    
                        placementData.RemoveCellObject(selectionResult.selectedGridPositions[i]);
                    structurePlacer.RemoveObjectAt(index);
                }
            }

        }
        buildingState.RefreshSelection();
        OnToggleUndo?.Invoke(true);
    }

    /// <summary>
    /// Selects the first position that we will use as selection.
    /// Ex for BoxSelection it will be its starting point
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void HandleSelectionStarted()
    {
        if (input.IsInteractingWithUI())
            return;
        if (buildingState == null)
            throw new Exception("buildingState is null. Check if it is initialized.");
        buildingState.HandleSelectionStarted(input.GetSelectedMapPosition());
    }

    /// <summary>
    /// Processes the selected positions when we let go of our mouse button
    /// </summary>
    private void HandleSelectionFinished()
    {
        if (buildingState == null)
            return;
        //Clear selection when we let go of the mouse button while over UI element
        //Prevents selection from changing while we are not pressing the mouse button (because of UI)
        if (input.IsInteractingWithUI())
        {
            buildingState.SelectionData.Clear();
        }
        buildingState.HandleSelectionFinished();
    }



    // Update is called once per frame
    void Update()
    {
        if (buildingState == null || input.IsInteractingWithUI())
            return;

        buildingState.HandleSelectionChanged(input.GetSelectedMapPosition());
    }

    // I don't like using Regions as the obscrure the readibility but the movement logic connects Removing and Placement state
    // so with the current implementation there was no way to include it inside the State pattern that is implemented right now.
    // It is part of the code that could be improved by refactoring the State pattern implementation (abstract BuildingState) 
    // to be even more high level. This also could be a separate class
    #region Move Objects Functionality

    private bool movement = false;
    Vector3Int? previousPosition;
    Quaternion previousRotation;

    /// <summary>
    /// Allows us to enter the movement state
    /// </summary>
    public void HandleMoveObject()
    {
        if (movement || buildingState is RemovingState)
        {
            return;
        }
        OnMovementStateEntered?.Invoke();
        CancelState();
        previousPosition = null;
        previousRotation = Quaternion.identity;
        gridManager.ToggleGrid(true);
        movement = true;
        input.OnCancle += CancelMovement;
        input.OnMousePressed += TrySelectingObjectToMove;
    }

    /// <summary>
    /// Tests if we have selected a valid object for movement. If not allows you to try again.
    /// If so removes the old objects and transfers you to a correct object placement state
    /// </summary>
    private void TrySelectingObjectToMove()
    {
        if (input.IsInteractingWithUI())
            return;
        Vector3 selectedPosition = input.GetSelectedMapPosition();
        previousPosition = gridManager.GetCellPosition(selectedPosition, PlacementType.FreePlacedObject);
        
        //Check if we have clicked on the selection grid or outside of it
        if (previousPosition.HasValue == false)
        {
            Debug.Log("Clicked outside of the selection grid");
            return;
        }
        itemData = structuresData.GetItemWithID(gridData.ObjectPlacementData.GetStructureIDForCellObject(previousPosition.Value));
        if (itemData != null) 
        {
            if (itemData.isMapObject)
            {
                Debug.Log("Trying to move map object");
                return;
            }
        }
        
        //Get the correct placemet position (important for bigger X direction size)
        previousPosition = gridData.ObjectPlacementData.GetOriginForCellObject(previousPosition.Value);
        if (previousPosition.HasValue == false || gridData.ObjectPlacementData.IsCellObjectAt(previousPosition.Value) == false)
        {
            previousPosition = null;
            Debug.Log("Nothing here");
        }
        else
        {
            
            Debug.Log("Selected a ");
            input.OnMousePressed -= TrySelectingObjectToMove; 
            SelectionResult result = new SelectionResult
            {
                isEdgeStructure = false,
                placementValidity = true,
                selectedGridPositions = new List<Vector3Int> { previousPosition.Value },
                selectedPositions = new List<Vector3> { gridManager.GetWorldPosition(previousPosition.Value)},
                selectedPositionGridCheckRotation = new List<Quaternion> { Quaternion.identity },
                selectedPositionsObjectRotation = new List<Quaternion> { Quaternion.identity }
            };
            itemData = structuresData.GetItemWithID(gridData.ObjectPlacementData.GetStructureIDForCellObject(previousPosition.Value));


            int gameObjectIndex = gridData.ObjectPlacementData.GetIndexForCellObject(previousPosition.Value);
            previousRotation = structurePlacer.GetObjectsRotation(gameObjectIndex);

            Debug.Log($"Selected a {itemData.name}");
            buildingState = new PlacingObjectsState(gridManager, gridData, itemData);
            buildingState.SelectionData.Rotation = previousRotation;
            TryRemovingObject(result);

            //Prepare the MovementState
            buildingState.OnFinished += (selectionResult) =>
            {
                TryPlacingObjects(selectionResult);
                previousPosition = null;
            };
            placementPrevew.StartShowingPreview(itemData.previewObject);
            buildingState.OnSelectionChanged += MovePreview;
            input.OnMouseReleased += TryMovingObject;


        }
    }

    /// <summary>
    /// Connects the input to a PlacingObjectsState while removing the connections to movement logic
    /// </summary>
    private void TryMovingObject()
    {
        input.OnMouseReleased -= TryMovingObject;
        ConnectInputToBuildingState();
        buildingState.OnFinished += (_) => CancelMovement();
    }

    /// <summary>
    /// Allows us to exit the movement logic and place the object back to its original spot if needed
    /// </summary>
    public void CancelMovement()
    {
        if (movement == false)
        {
            return;
        }
        if (previousPosition != null)
            PlaceMovedObjectBackinPlace();
        CancelState();
        gridManager.ToggleGrid(false);
        OnExitMovement?.Invoke();
        movement = false;
        input.OnCancle -= CancelMovement;
        input.OnMousePressed -= TrySelectingObjectToMove;
    }

    /// <summary>
    /// Places objet back in its original position
    /// </summary>
    private void PlaceMovedObjectBackinPlace()
    {
        SelectionResult result = new SelectionResult
        {
            isEdgeStructure = false,
            placementValidity = true,
            selectedGridPositions = new List<Vector3Int> { previousPosition.Value },
            selectedPositions = new List<Vector3> { gridManager.GetWorldPosition(previousPosition.Value) },
            selectedPositionGridCheckRotation = new List<Quaternion> { previousRotation },
            selectedPositionsObjectRotation = new List<Quaternion> { previousRotation }
        };
        TryPlacingObjects(result);
        previousPosition = null;
    }
    #endregion
}
