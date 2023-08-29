using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IconSelector 
{
    
    
    
    // --------------- TO SELECT THE UTILITY OF EVERY ICONS OF THE MAP ---------------
    public void ChoseIcons(List<ListSpots> map, AventureData data, int startRaw)
    {
        for (int y = startRaw; y < map.Count; y++)
        {
            for (int x = 0; x < map[y].list.Count; x++)
            {
                if (map[y].list[x] is not null)
                {
                    Nod currentNod = map[y].list[x];
                    currentNod.previousNods = new List<Nod>();
                    currentNod.nextNods = new List<Nod>();

                    SeparateNeighbors(currentNod.connectedNods, currentNod.previousNods, currentNod.nextNods, currentNod);


                    if (currentNod.isCamp)
                    {
                        currentNod.InitialiseNode(Nod.NodeType.camp, 0, y);

                        if (currentNod.previousNods.Count != 0) 
                        {
                            List<List<Nod>> possiblePaths = RecursivePaths(currentNod.previousNods, currentNod);

                            CheckPaths(map, possiblePaths, data);
                        }
                    }
                    
                    else if (currentNod.previousNods[0].isCamp)
                    {
                        currentNod.nodeType = Nod.NodeType.battle;
                        currentNod.InitialiseNode(Nod.NodeType.battle, 2, y);
                    }
                    
                    else if (y == map.Count - 1)
                    {
                        currentNod.nodeType = Nod.NodeType.chest;
                        currentNod.InitialiseNode(Nod.NodeType.chest, 0, y);
                        
                        List<List<Nod>> possiblePaths = RecursivePaths(currentNod.previousNods, currentNod);
                        CheckPaths(map, possiblePaths, data);
                    }

                    else
                    {
                        SelectRandomIcon(map, data, currentNod, y, data.nodeTypes);
                    }
                }
            }
        }
    }


    private void SeparateNeighbors(List<Nod> neighbors, List<Nod> previousNeighbors, List<Nod> nextNeighbors, Nod currentNod)
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (neighbors[i].transform.position.x < currentNod.transform.position.x)
            {
                previousNeighbors.Add(neighbors[i]);
            }
            else
            {
                nextNeighbors.Add(neighbors[i]);
            }
        }
    }

    private List<List<Nod>> RecursivePaths(List<Nod> previousNods, Nod currentNod)
    {
        List<List<Nod>> currentPaths = new List<List<Nod>>();

        for (int i = 0; i < previousNods.Count; i++)
        {
            if (!previousNods[i].isCamp)
            {
                List<List<Nod>> previousPaths = RecursivePaths(previousNods[i].previousNods, previousNods[i]);

                for (int j = 0; j < previousPaths.Count; j++)
                {
                    currentPaths.Add(previousPaths[j]);
                    currentPaths[currentPaths.Count - 1].Add(currentNod);
                }
            }
            else
            {
                currentPaths.Add(new List<Nod>());
                currentPaths[currentPaths.Count - 1].Add(currentNod);
            }
        }

        return currentPaths;
    }

    
    
    // ---------------------------- BALANCE PART ----------------------------------
    
    
    // CHECKS EVERY PREVIOUS PATHS TO BALANCE THEM
    private void CheckPaths(List<ListSpots> map, List<List<Nod>> currentPaths, AventureData data)
    {
        bool isOkay = false;
        int iterations = 0;

        while (!isOkay)
        {
            for (int i = 0; i < currentPaths.Count; i++)
            {
                int pathDifficulty = GetPathDifficulty(currentPaths[i]);

                if (pathDifficulty > 3 || pathDifficulty < 0)
                {
                    List<NodeTypeClass> downNodes = new List<NodeTypeClass>();
                    List<NodeTypeClass> upNodes = new List<NodeTypeClass>();

                    for (int j = 0; j < data.nodeTypes.Count; j++)
                    {
                        if (data.nodeTypes[j].difficultyValue > 0)
                        {
                            upNodes.Add(data.nodeTypes[j]);
                        }
                        else if (data.nodeTypes[j].difficultyValue < 0)
                        {
                            downNodes.Add(data.nodeTypes[j]);
                        }
                    }

                    BalancePath(map, data, currentPaths[i], pathDifficulty, upNodes, downNodes);
                }
            }

            isOkay = true;
            
            for (int i = 0; i < currentPaths.Count; i++)
            {
                int pathDifficulty = GetPathDifficulty(currentPaths[i]);

                if (pathDifficulty > 3 || pathDifficulty < 0)
                {
                    isOkay = false;
                }
            }

            iterations += 1;
            if (iterations > 20)
            {
                isOkay = true;
            }
        }
    }

    
    // RETURN THE DIFFICULTY VALUE OF A PATH
    private int GetPathDifficulty(List<Nod> currentPath)
    {
        int currentDifficulty = 0;
        
        for (int i = 0; i < currentPath.Count; i++)
        {
            currentDifficulty += currentPath[i].nodeDifficulty;
        }

        return currentDifficulty;
    }


    private void BalancePath(List<ListSpots> map, AventureData data, List<Nod> currentPath, int currentDifficulty, List<NodeTypeClass> upNodes, List<NodeTypeClass> downNodes)
    {
        bool isOkay = false;

        while (!isOkay)
        {
            int indexNode = Random.Range(1, currentPath.Count - 1);
            List<NodeTypeClass> newNodeType = new List<NodeTypeClass>();

            if (currentDifficulty > 3)
            {
                newNodeType = downNodes;
            }
            else
            {
                newNodeType = upNodes;
            }
            
            //currentPath[indexNode].InitialiseNode(newNodeType.nodType, newNodeType.difficultyValue);

            SelectRandomIcon(map, data, currentPath[indexNode], currentPath[indexNode].mapY, newNodeType);

            currentDifficulty = GetPathDifficulty(currentPath);

            if (currentDifficulty <= 3 && currentDifficulty >= 0)
            {
                isOkay = true;
            }
        }
    }
    
    

    // ---------------------------- RANDOM PART ----------------------------------
    
    private void SelectRandomIcon(List<ListSpots> map, AventureData data, Nod currentNod, int y, List<NodeTypeClass> nodeTypes)
    {
        bool isOkay = false;

        while (!isOkay)
        {
            Nod.NodeType selectedNodeType = Nod.NodeType.none;
            int selectedNodStart = 0;
            int selectedNodeDifficulty = 0;

            List<NodeTypeClass> currentNodeTypes = new List<NodeTypeClass>(nodeTypes);

            for (int i = currentNodeTypes.Count - 1; i >= 0; i--)
            {
                int draw = Random.Range(0, 100);
                int index = Random.Range(0, currentNodeTypes.Count);

                if (draw < currentNodeTypes[index].percentageSpawn)
                {
                    selectedNodeType = currentNodeTypes[index].nodType;
                    selectedNodeDifficulty = currentNodeTypes[index].difficultyValue;
                    selectedNodStart = currentNodeTypes[index].startSpawn;

                    break;
                }

                else
                {
                    currentNodeTypes.RemoveAt(index);
                }
            }

            if (selectedNodeType != Nod.NodeType.none)
            {
                bool test1 = VerifyConsecutive(currentNod, selectedNodeType);
                bool test2 = VerifyChoice(map, y, currentNod, selectedNodeType);
                bool test3 = VerifyStart(data, y, selectedNodStart);

                if (test1 && test2 && test3)
                {
                    currentNod.InitialiseNode(selectedNodeType, selectedNodeDifficulty, y);
                                
                    isOkay = true;
                }
            }
        }
        
    }
    

    private bool VerifyConsecutive(Nod currentNod, Nod.NodeType wantedNodeType)
    {
        if (wantedNodeType != Nod.NodeType.battle && wantedNodeType != Nod.NodeType.events)
        {
            for (int i = 0; i < currentNod.connectedNods.Count; i++)
            {
                Nod currentNeighbor = currentNod.connectedNods[i];

                if (currentNeighbor.nodeType != Nod.NodeType.none)
                {
                    if (currentNeighbor.nodeType == wantedNodeType)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private bool VerifyChoice(List<ListSpots> map, int currentY, Nod currentNod, Nod.NodeType wantedNodeType)
    {
        if (wantedNodeType == Nod.NodeType.battle)
        {
            return true;
        }
        
        for (int x = 0; x < map[currentY].list.Count; x++)
        {
            if (map[currentY].list[x] is not null)
            {
                if (map[currentY].list[x] != currentNod)
                {
                    if (VerifySamePrevious(currentNod, map[currentY].list[x]))
                    {
                        if (map[currentY].list[x].nodeType == wantedNodeType)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }
    
    private bool VerifySamePrevious(Nod nod1, Nod nod2)
    {
        bool hasSamePrevious = false;

        for (int i = 0; i < nod1.connectedNods.Count; i++)
        {
            for (int j = 0; j < nod2.connectedNods.Count; j++)
            {
                if (nod1.connectedNods[i] == nod2.connectedNods[j])
                {
                    if (nod1.connectedNods[i].transform.position.x < nod1.transform.position.x)
                    {
                        hasSamePrevious = true;
                    }
                }
            }
        }
        
        return hasSamePrevious;
    }
    
    private bool VerifyStart(AventureData data, int currentY, int nodStartPercentage)
    {
        if (nodStartPercentage != 0)
        {
            float currentRatioMap = (float)currentY / (float) data.wantedMapLength;
            float nodRatio = (float)nodStartPercentage / 100f;
            
            if (currentRatioMap > nodRatio)
            {
                return true;
            }

            return false;
        }

        return true;
    }
}
