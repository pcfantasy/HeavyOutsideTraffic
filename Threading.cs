using ColossalFramework;
using ColossalFramework.Globalization;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HeavyOutsideTraffic
{
    public class HeavyOutsideTrafficThreading : ThreadingExtensionBase
    {
        public override void OnAfterSimulationFrame()
        {
            base.OnAfterSimulationFrame();
            if (Loader.CurrentLoadMode == LoadMode.LoadGame || Loader.CurrentLoadMode == LoadMode.NewGame)
            {
                if (HeavyOutsideTraffic.IsEnabled)
                {
                    uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
                    int num4 = (int)(currentFrameIndex & 255u);
                    int num5 = num4 * 192;
                    int num6 = (num4 + 1) * 192 - 1;
                    //DebugLog.LogToFileOnly("currentFrameIndex num2 = " + currentFrameIndex.ToString());
                    BuildingManager instance = Singleton<BuildingManager>.instance;

                    if (num4 == 255)
                    {
                        RoadUI.refeshOnce = true;
                    }

                    for (int i = num5; i <= num6; i = i + 1)
                    {
                        if (instance.m_buildings.m_buffer[i].Info.m_buildingAI is OutsideConnectionAI)
                        {
                            if (instance.m_buildings.m_buffer[i].Info.m_class.m_service == ItemClass.Service.Road)
                            {
                                ProcessDummyTraffic((ushort)i, ref instance.m_buildings.m_buffer[i]);
                            }
                        }
                    }
                }
            }
        }

        public static float checkTrafficLevel(ushort buildingID, ref Building data)
        {
            ushort num = data.FindParentNode(buildingID);
            NetManager instance = Singleton<NetManager>.instance;
            float roadIdex = 1f;
            int wayCount = 0;
            for (int i = 0; i < 8; i++)
            {
                ushort segment = instance.m_nodes.m_buffer[(int)num].GetSegment(i);
                if (segment != 0)
                {
                    NetInfo info = instance.m_segments.m_buffer[(int)segment].Info;
                    //DebugLog.LogToFileOnly("outside connection building is " + info.m_lanes.Length.ToString());
                    for (int j = 0; j < info.m_lanes.Length; j++)
                    {
                        //DebugLog.LogToFileOnly("outside connection lane is " + info.m_lanes[j].m_direction.ToString());
                        if (info.m_lanes[j].m_direction != NetInfo.Direction.None)
                        {
                            wayCount++;
                        }
                    }
                    break;
                }
            }

            if (wayCount !=0)
            {
                roadIdex = wayCount / 3f;
            }

            if (((data.m_flags & Building.Flags.Incoming) != Building.Flags.None) && ((data.m_flags & Building.Flags.Outgoing) != Building.Flags.None))
            {
                roadIdex = roadIdex * 0.5f;
            }

            SimulationManager instance2 = Singleton<SimulationManager>.instance;

            if (data.m_position.x > 8600)
            {
                    return (float)(HeavyOutsideTraffic.aTraffic + ((float)instance2.m_randomizer.Int32(40) / 100f)) * roadIdex;
            }
            else if (data.m_position.z > 8600)
            {
                    return (float)(HeavyOutsideTraffic.bTraffic + ((float)instance2.m_randomizer.Int32(40) / 100f)) * roadIdex;
            }
            else if (data.m_position.x < -8600)
            {
                    return (float)(HeavyOutsideTraffic.cTraffic + ((float)instance2.m_randomizer.Int32(40) / 100f)) * roadIdex;
            }
            else if (data.m_position.z < -8600)
            {
                    return (float)(HeavyOutsideTraffic.dTraffic + ((float)instance2.m_randomizer.Int32(40) / 100f)) * roadIdex;
            }
            else
            {
                return 1f;
            }
        }


        public void ProcessDummyTraffic(ushort buildingID, ref Building data)
        {
            
            SimulationManager instance = Singleton<SimulationManager>.instance;
            TransferManager instance2 = Singleton<TransferManager>.instance;
            uint vehicleCount = (uint)Singleton<VehicleManager>.instance.m_vehicleCount;
            uint instanceCount = (uint)Singleton<CitizenManager>.instance.m_instanceCount;
            float num = 0;
            if (vehicleCount * 65536u > instanceCount * 16384u)
            {
                num = (float)(16384f - vehicleCount)/ 16384f;
                //DebugLog.LogToFileOnly("vehicleCount = " + vehicleCount.ToString());
            }
            else
            {
                num = (float)(65536f - instanceCount) / 65536f;
                //DebugLog.LogToFileOnly("instanceCount = " + instanceCount.ToString());
            }

            if (num <= 0)
            {
                num = 0;
            }

            //DebugLog.LogToFileOnly("num = " + num.ToString());

            float roadTrafficIdex = checkTrafficLevel(buildingID, ref data);
            int roadTraffic = 0;
            roadTraffic = (int)(6f * roadTrafficIdex * num + ((float)instance.m_randomizer.Int32(100) / 100f));

            //DebugLog.LogToFileOnly("roadTraffic = " + roadTraffic.ToString());
            if (((data.m_flags & Building.Flags.Incoming) != Building.Flags.None)  && ((data.m_flags & Building.Flags.Outgoing) != Building.Flags.None))
            {
                TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveIncomingOffer(TransferManager.TransferReason.DummyCar, offer);
                Singleton<TransferManager>.instance.RemoveOutgoingOffer(TransferManager.TransferReason.DummyCar, offer);

                TransferManager.TransferOffer offer2 = default(TransferManager.TransferOffer);
                offer2.Building = buildingID;
                offer2.Position = data.m_position * ((float)instance.m_randomizer.Int32(100, 400) * 0.01f);
                offer2.Active = false;
                offer2.Priority = 7;

                if (roadTraffic > 0)
                {
                    offer2.Amount = roadTraffic;
                    instance2.AddIncomingOffer(TransferManager.TransferReason.DummyCar, offer2);
                    offer2.Active = true;
                    instance2.AddOutgoingOffer(TransferManager.TransferReason.DummyCar, offer2);
                }
            }
            else if ((data.m_flags & Building.Flags.Incoming) != Building.Flags.None)
            {
                TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveIncomingOffer(TransferManager.TransferReason.DummyCar, offer);

                TransferManager.TransferOffer offer2 = default(TransferManager.TransferOffer);
                offer2.Building = buildingID;
                offer2.Position = data.m_position * ((float)instance.m_randomizer.Int32(100, 400) * 0.01f);
                offer2.Active = false;
                offer2.Priority = 7;
                if (roadTraffic > 0)
                {
                    offer2.Amount = roadTraffic;
                    instance2.AddIncomingOffer(TransferManager.TransferReason.DummyCar, offer2);
                }

            }
            else
            {
                TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveOutgoingOffer(TransferManager.TransferReason.DummyCar, offer);


                TransferManager.TransferOffer offer2 = default(TransferManager.TransferOffer);
                offer2.Building = buildingID;
                offer2.Position = data.m_position * ((float)instance.m_randomizer.Int32(100, 400) * 0.01f);
                offer2.Active = true;
                offer2.Priority = 7;
                if (roadTraffic > 0)
                {
                    offer2.Amount = roadTraffic;
                    instance2.AddOutgoingOffer(TransferManager.TransferReason.DummyCar, offer2);
                }
            }

            //DebugLog.LogToFileOnly("outside connection building is " + buildingID.ToString() + data.m_flags.ToString());
        }

    }
}
