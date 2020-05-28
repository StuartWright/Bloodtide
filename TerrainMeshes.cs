using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TerrainMeshes : MonoBehaviour
{
    public GameObject Plant, Plant2;
    RaycastHit Hit;
    void Start()
    {
        Stopwatch st = new Stopwatch();
        st.Start();
        for (float i = -40; i < 240; i += 0.5f)
        {
            for(float j = -60; j < 220; j+= 0.5f)
            {
                if(Physics.Raycast(transform.position = new Vector3(i, 10, j), Vector3.down, out Hit, 11))
                {
                    if (Hit.collider.name == "Terrain")
                    {
                        int y = Random.Range(0, 500);
                        if (y < 1)
                        {
                            int t = Random.Range(0, 2);
                            switch (t)
                            {
                                case 0:
                                    Instantiate(Plant, new Vector3(i, 0, j), transform.rotation);
                                    break;
                                case 1:
                                    Instantiate(Plant2, new Vector3(i, 0, j), Plant2.transform.localRotation = Quaternion.Euler(-90, 0, 0));
                                    break;
                            }
                        }
                    }                       
                }
                              
            }
        }
        st.Stop();
        print("Terrain Build: " + st.Elapsed);
    }
}


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMeshes : MonoBehaviour
{
    public GameObject Plant, Plant2, House, one, two, three, four, five;
    RaycastHit Hit;
    void Start()
    {
        for (float i = -40; i < 150; i += 0.3f)
        {
            for (float j = -60; j < 130; j += 0.3f)
            {
                if (Physics.Raycast(transform.position = new Vector3(i, 10, j), Vector3.down, out Hit, 11))
                {
                    if (Hit.collider.name == "Terrain")
                    {
                        int y = Random.Range(0, 5000);

                        if (y == 0)
                        {
                            int u = Random.Range(0, 6);
                            switch (u)
                            {
                                case 0:
                                    Instantiate(House, new Vector3(i, 0, j), House.transform.localRotation = Quaternion.Euler(-90, 0, RandomRotation()));
                                    break;
                                case 1:
                                    Instantiate(one, new Vector3(i, 0, j), one.transform.localRotation = Quaternion.Euler(-90, 0, RandomRotation()));
                                    break;
                                case 2:
                                    Instantiate(two, new Vector3(i, 0, j), two.transform.localRotation = Quaternion.Euler(-90, 0, 0));
                                    break;
                                case 3:
                                    Instantiate(three, new Vector3(i, 0, j), three.transform.localRotation = Quaternion.Euler(-90, 0, RandomRotation()));
                                    break;
                                case 4:
                                    Instantiate(four, new Vector3(i, 0, j), four.transform.localRotation = Quaternion.Euler(-90, 0, 0));
                                    break;
                                case 5:
                                    Instantiate(five, new Vector3(i, 0, j), transform.rotation);
                                    break;

                            }
                        }
                        else if (y < 30)
                        {
                            int t = Random.Range(0, 2);
                            switch (t)
                            {
                                case 0:
                                    Instantiate(Plant, new Vector3(i, 0, j), transform.rotation);
                                    break;
                                case 1:
                                    Instantiate(Plant2, new Vector3(i, 0, j), Plant2.transform.localRotation = Quaternion.Euler(-90, 0, 0));
                                    break;
                            }
                        }
                    }
                }

            }
        }
    }
    int RandomRotation()
    {
        int i = Random.Range(0, 4);
        if (i == 0)
            return 0;
        else if (i == 1)
            return 90;
        else if (i == 2)
            return 180;
        else if (i == 3)
            return -90;
        else return 0;
    }

}
*/