using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CarcassonneCraft
{
    class Construction : MonoBehaviour
    {
        PanelScript panel;
        const float distance = 4.0f;

        int selectBlockType = 1;
        float timer = 0.0f;

        void Start()
        {
            panel = GameObject.Find("AreaInfoPanel").GetComponent<PanelScript>();
        }

        void Update()
        {
            if(!panel.IsPanelOpen())
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    float wheel = Input.GetAxis("Mouse ScrollWheel");
                    if (wheel > 0)
                    {
                        timer = 0.1f;

                        selectBlockType++;
                        if(selectBlockType == BlockTypes.GetMaxCount())
                        {
                            selectBlockType = 1;
                        }
                        //Debug.Log(selectBlockType);
                    }
                    else if (wheel < 0)
                    {
                        timer = 0.1f;

                        selectBlockType--;
                        if (selectBlockType == 0)
                        {
                            selectBlockType = BlockTypes.GetMaxCount() - 1;
                        }
                        //Debug.Log(selectBlockType);
                    }
                }
            }

            if (Input.GetButtonDown("Fire1"))
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position/* + transform.forward * 1.0f*/, transform.forward, out hit, distance))
                {
                    Vector3 hitPoint = hit.point + hit.normal * 0.1f;
                    int x = (int)hitPoint.x;
                    int y = (int)hitPoint.y;
                    int z = (int)hitPoint.z;

                    Collider[] cols = Physics.OverlapBox(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), new Vector3(0.49f, 0.49f, 0.49f));
                    if (cols.Length == 0)
                    {
                        if (Env.IsInsideWorld(x, y, z))
                        {
                            XZNum areasNum = Env.GetAreasNum(new XZNum(x, z));
                            XZNum chunkNum = Env.GetChunkNum(new XZNum(x, z));
                            XZNum blockNum = Env.GetBlockNum(new XZNum(x, z));
                            int areaid = Players.GetSelectArea(areasNum);

                            int defBlockType = Env.GetBlockType(x, y, z);
                            if (defBlockType == selectBlockType)
                            {
                                SetBlockInfo block = new SetBlockInfo();
                                block.areaid = areaid;
                                block.xareasnum = areasNum.xnum;
                                block.zareasnum = areasNum.znum;
                                block.xchunknum = chunkNum.xnum;
                                block.zchunknum = chunkNum.znum;
                                block.x = blockNum.xnum;
                                block.y = y;
                                block.z = blockNum.znum;
                                GCli.Send(MessageType.ResetBlock, GCli.Serialize<SetBlockInfo>(block), NetDeliveryMethod.ReliableOrdered);
                            }
                            else
                            {
                                SetBlockInfo block = new SetBlockInfo();
                                block.areaid = areaid;
                                block.xareasnum = areasNum.xnum;
                                block.zareasnum = areasNum.znum;
                                block.xchunknum = chunkNum.xnum;
                                block.zchunknum = chunkNum.znum;
                                block.x = blockNum.xnum;
                                block.y = y;
                                block.z = blockNum.znum;
                                block.blocktype = selectBlockType;
                                GCli.Send(MessageType.SetBlock, GCli.Serialize<SetBlockInfo>(block), NetDeliveryMethod.ReliableOrdered);
                            }
                        }
                    }
                }

            }
            else if (Input.GetButtonDown("Fire2"))
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position/* + transform.forward * 1.0f*/, transform.forward, out hit, distance))
                {
                    Vector3 hitPoint = hit.point - hit.normal * 0.1f;
                    int x = (int)hitPoint.x;
                    int y = (int)hitPoint.y;
                    int z = (int)hitPoint.z;
                    if (Env.IsInsideWorld(x, y, z))
                    {
                        XZNum areasNum = Env.GetAreasNum(new XZNum(x, z));
                        XZNum chunkNum = Env.GetChunkNum(new XZNum(x, z));
                        XZNum blockNum = Env.GetBlockNum(new XZNum(x, z));
                        int areaid = Players.GetSelectArea(areasNum);

                        //Debug.Log("World:" + x + "," + y + "," + z);
                        //Debug.Log("Relative:" + blockNum.xnum + "," + y + "," + blockNum.znum);

                        int defBlockType = Env.GetBlockType(x, y, z);
                        if (defBlockType == 0)
                        {
                            SetBlockInfo block = new SetBlockInfo();
                            block.areaid = areaid;
                            block.xareasnum = areasNum.xnum;
                            block.zareasnum = areasNum.znum;
                            block.xchunknum = chunkNum.xnum;
                            block.zchunknum = chunkNum.znum;
                            block.x = blockNum.xnum;
                            block.y = y;
                            block.z = blockNum.znum;
                            GCli.Send(MessageType.ResetBlock, GCli.Serialize<SetBlockInfo>(block), NetDeliveryMethod.ReliableOrdered);
                        }
                        else
                        {
                            SetBlockInfo block = new SetBlockInfo();
                            block.areaid = areaid;
                            block.xareasnum = areasNum.xnum;
                            block.zareasnum = areasNum.znum;
                            block.xchunknum = chunkNum.xnum;
                            block.zchunknum = chunkNum.znum;
                            block.x = blockNum.xnum;
                            block.y = y;
                            block.z = blockNum.znum;
                            block.blocktype = 0;
                            GCli.Send(MessageType.SetBlock, GCli.Serialize<SetBlockInfo>(block), NetDeliveryMethod.ReliableOrdered);
                        }
                    }
                }
            }
        }
    }
}
