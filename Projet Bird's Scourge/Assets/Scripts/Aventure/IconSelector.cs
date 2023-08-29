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

                    if (!currentNod.isCamp)
                    {
                        List<List<Nod>> possiblePaths = RecursivePaths(currentNod.previousNods, currentNod);
                        Debug.Log(possiblePaths.Count);
                    }


                    if (currentNod.isCamp)
                    {
                        currentNod.nodeType = Nod.NodeType.camp;
                        currentNod.InitialiseNode();
                    }
                    
                    else if (currentNod.previousNods[0].isCamp)
                    {
                        currentNod.nodeType = Nod.NodeType.battle;
                        currentNod.InitialiseNode();
                    }
                    
                    else if (y == map.Count - 1)
                    {
                        currentNod.nodeType = Nod.NodeType.chest;
                        currentNod.InitialiseNode();
                    }

                    else
                    {
                        SelectRandomIcon(map, data, currentNod, y);
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

    /*private List<List<Nod>> FindPossiblePaths(List<Nod> previousNods)
    {
        List<List<Nod>> currentPaths = new List<List<Nod>>();

        for (int i = 0; i < previousNods.Count; i++)
        {
            
        }
        
        return null;
    }*/

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


    private void SelectRandomIcon(List<ListSpots> map, AventureData data, Nod currentNod, int y)
    {
        bool isOkay = false;

        while (!isOkay)
        {
            Nod.NodeType selectedNodeType = Nod.NodeType.none;
            int selectedNodStart = 0;

            List<NodeTypeClass> currentNodeTypes = new List<NodeTypeClass>(data.nodeTypes);

            for (int i = currentNodeTypes.Count - 1; i >= 0; i--)
            {
                int draw = Random.Range(0, 100);
                int index = Random.Range(0, currentNodeTypes.Count);

                if (draw < currentNodeTypes[index].percentageSpawn)
                {
                    selectedNodeType = currentNodeTypes[index].nodType;
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
                    currentNod.nodeType = selectedNodeType;
                    currentNod.InitialiseNode();
                                
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
