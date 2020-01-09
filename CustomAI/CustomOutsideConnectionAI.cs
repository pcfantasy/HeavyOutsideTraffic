using ColossalFramework;
using HeavyOutsideTraffic.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeavyOutsideTraffic.CustomAI
{
    public class CustomOutsideConnectionAI
    {
        public static void OutsideConnectionAISimulationStepPostFix(ushort buildingID, ref Building data)
        {
            if (data.Info.m_class.m_service == ItemClass.Service.Road)
            {
                ProcessDummyTraffic(buildingID, ref data);
            }
        }

        public static float checkTrafficLevel(ushort buildingID, ref Building data)
        {
            ushort num = data.FindParentNode(buildingID);
            NetManager instance = Singleton<NetManager>.instance;
            float roadIdex = 1f;
            int wayCount = 0;
            NetInfo info = instance.m_nodes.m_buffer[(int)num].Info;
            for (int j = 0; j < info.m_lanes.Length; j++)
            {
                if (info.m_lanes[j].m_laneType.IsFlagSet(NetInfo.LaneType.Vehicle) && info.m_lanes[j].m_vehicleType.IsFlagSet(VehicleInfo.VehicleType.Car))
                {
                    wayCount++;
                }
            }

            if (wayCount != 0)
            {
                roadIdex = wayCount / 3f;
            }

            if (((data.m_flags & Building.Flags.Incoming) != Building.Flags.None) && ((data.m_flags & Building.Flags.Outgoing) != Building.Flags.None))
            {
                roadIdex *= 0.5f;
            }

            SimulationManager instance2 = Singleton<SimulationManager>.instance;

            if (data.m_position.x > 8600)
            {
                return (float)(HeavyOutsideTraffic.aTraffic + ((float)instance2.m_randomizer.Int32(50) / 100f)) * roadIdex;
            }
            else if (data.m_position.z > 8600)
            {
                return (float)(HeavyOutsideTraffic.bTraffic + ((float)instance2.m_randomizer.Int32(50) / 100f)) * roadIdex;
            }
            else if (data.m_position.x < -8600)
            {
                return (float)(HeavyOutsideTraffic.cTraffic + ((float)instance2.m_randomizer.Int32(50) / 100f)) * roadIdex;
            }
            else if (data.m_position.z < -8600)
            {
                return (float)(HeavyOutsideTraffic.dTraffic + ((float)instance2.m_randomizer.Int32(50) / 100f)) * roadIdex;
            }
            else
            {
                return 1f;
            }
        }


        public static void ProcessDummyTraffic(ushort buildingID, ref Building data)
        {

            SimulationManager instance = Singleton<SimulationManager>.instance;
            TransferManager instance2 = Singleton<TransferManager>.instance;
            uint vehicleCount = (uint)Singleton<VehicleManager>.instance.m_vehicleCount;
            uint instanceCount = (uint)Singleton<CitizenManager>.instance.m_instanceCount;
            float num = 0;
            if (vehicleCount * 65536u > instanceCount * 16384u)
            {
                num = (float)(16384f - vehicleCount) / 16384f;
            }
            else
            {
                num = (float)(65536f - instanceCount) / 65536f;
            }

            if (num <= 0)
            {
                num = 0;
            }

            float roadTrafficIdex = checkTrafficLevel(buildingID, ref data);
            int roadTraffic = 0;
            roadTraffic = (int)(8f * roadTrafficIdex * num + ((float)instance.m_randomizer.Int32(100) / 100f));

            if (((data.m_flags & Building.Flags.Incoming) != Building.Flags.None) && ((data.m_flags & Building.Flags.Outgoing) != Building.Flags.None))
            {
                TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveIncomingOffer(TransferManager.TransferReason.DummyCar, offer);
                Singleton<TransferManager>.instance.RemoveOutgoingOffer(TransferManager.TransferReason.DummyCar, offer);
                TransferManager.TransferOffer offer2 = default(TransferManager.TransferOffer);
                offer2.Building = buildingID;
                offer2.Position = data.m_position * ((float)instance.m_randomizer.Int32(100, 400) * 0.01f);
                offer2.Active = false;
                offer2.Priority = instance.m_randomizer.Int32(0, 7);
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
                offer2.Priority = instance.m_randomizer.Int32(0, 7);
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
                offer2.Priority = instance.m_randomizer.Int32(0, 7);
                if (roadTraffic > 0)
                {
                    offer2.Amount = roadTraffic;
                    instance2.AddOutgoingOffer(TransferManager.TransferReason.DummyCar, offer2);
                }
            }
        }
    }
}
