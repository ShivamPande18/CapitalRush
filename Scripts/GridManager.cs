using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public List<GameObject> cubes = new List<GameObject>();
    public List<int> currSkips = new List<int>();
    public List<int> nextSkips = new List<int>();

    public CameraSrc camSrc;
    public GameObject cube;
    public Material redMat;

    public int side = 2;

    readonly float xBorder = 1f;
    readonly float yBorder = .5f;

    private void Start()
    {
        side = 2;
        UpdateGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            side++;
            UpdateGrid();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            side--;
            UpdateGrid();
        }

    }

    void UpdateGrid()
    {
        if (side < 2)
        {
            side = 2; //min side length
            return;
        }

        if (side > 10)
        {
            side = 10;  //max side length
            return;
        }

        camSrc.OnSideChange(side);  //Cam position update


        //Remove all previous cubes in the grid
        int last = cubes.Count - 1;

        if (last != -1)
        {
            for (int i = last; i >= 0; i--)
            {
                Destroy(cubes[i]);
                cubes.RemoveAt(i);
            }
        }


        bool ySkip = false;

        for (int x = 0; x < side; x++)
        {
            currSkips.AddRange(nextSkips);  // nextSkips Takes in account what cordinates are occupied by oversized child components 
            nextSkips.Clear();

            for (int y = 0; y < side; y++)
            {
                GameObject cubeGo = Instantiate(cube, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                Transform childObj = cubeGo.transform.GetChild(0);
                float childScaleX = 0;
                float childScaleY = 0;

                if (Random.Range(0, 10) % 2 == 0) childScaleX = Random.Range(.5f, 1.5f); //Makes the cube change size in either x or y dir at random
                else childScaleY = Random.Range(.25f, .75f);

                if (currSkips.Count > 0 && currSkips[0] == y)
                {
                    // Executed if the current cordinate is overlapped by bigger child of other cube, so it changes color and removes its child

                    cubeGo.transform.GetChild(0).gameObject.SetActive(false);
                    cubeGo.GetComponent<Renderer>().material = redMat;
                    currSkips.RemoveAt(0);
                }
                else if (ySkip)
                {
                    // Executed if the current cordinate is overlapped by bigger child of cube below, so it changes color and removes its child

                    cubeGo.transform.GetChild(0).gameObject.SetActive(false);
                    cubeGo.GetComponent<Renderer>().material = redMat;
                    ySkip = false;
                }
                else
                {

                    if (childScaleY == 0)
                    {
                        //If changing size in x dir

                        if (x < side - 1)
                        {
                            childObj.localScale = new Vector3(childScaleX, childObj.localScale.y, 0.5f);    //Random scale assignment
                            if (childScaleX > xBorder)
                            {
                                //Avoids enlargment of border cubes

                                cubeGo.GetComponent<Renderer>().material = redMat;
                                nextSkips.Add(y);
                                childObj.position += new Vector3(0.5f, 0, 0);
                            }
                        }
                    }
                    else
                    {
                        //If changing size in y dir


                        if (y < side - 1 && CanOccupy(y+1))
                        {
                            childObj.localScale = new Vector3(childObj.localScale.x, childScaleY, 0.5f);    //Random scale assignment

                            if (childScaleY > yBorder)
                            {
                                //Avoids enlargment of border cubes

                                cubeGo.GetComponent<Renderer>().material = redMat;
                                childObj.position += new Vector3(0, 0.5f, 0);
                                ySkip = true;
                            }
                        }
                    }

                }
                cubes.Add(cubeGo);
            }
        }
    }

    bool CanOccupy(int cord)
    {

        //Checks for occupancy in any y cordinate of current x cordinate

        if (currSkips.Count == 0) return true;
        if (currSkips[0] == cord) return false;
        return true;
    }
}
