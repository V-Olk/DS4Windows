﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using DS4Windows;

namespace DS4WinWPF.DS4Control.DTOXml
{
    [XmlRoot("Actions")]
    public class ActionsDTO : IDTO<BackingStore>
    {
        [XmlElement("Action")] // Use XmlElement here to skip container element
        public List<SpecialActionSerializer> actionSerializers;

        public ActionsDTO()
        {
            actionSerializers = new List<SpecialActionSerializer>();
        }

        public void MapFrom(BackingStore source)
        {
            foreach(SpecialAction action in source.actions)
            {
                SpecialActionSerializer actionSerializer = new SpecialActionSerializer();
                switch(action.typeID)
                {
                    case SpecialAction.ActionTypeId.Macro:
                        actionSerializer.TypeString = action.type;
                        actionSerializer.Details = action.details;
                        actionSerializer.Extras = action.extra;
                        break;
                    case SpecialAction.ActionTypeId.Program:
                        actionSerializer.TypeString = action.type;
                        actionSerializer.Details = action.details;
                        actionSerializer.Arguments = action.extra;
                        actionSerializer.Delay = action.delayTime;
                        break;
                    case SpecialAction.ActionTypeId.Profile:
                        actionSerializer.TypeString = action.type;
                        actionSerializer.Details = action.details;
                        actionSerializer.UnloadTrigger = action.extra;
                        break;

                    case SpecialAction.ActionTypeId.Key:
                        string[] exts = action.extra.Split('\n');
                        actionSerializer.TypeString = action.type;
                        actionSerializer.Details = action.details;
                        actionSerializer.UnloadStyle = exts[0];
                        actionSerializer.UnloadTrigger = exts[1];
                        break;

                    case SpecialAction.ActionTypeId.DisconnectBT:
                    case SpecialAction.ActionTypeId.BatteryCheck:
                    case SpecialAction.ActionTypeId.MultiAction:
                    case SpecialAction.ActionTypeId.SASteeringWheelEmulationCalibrate:
                        actionSerializer.TypeString = action.type;
                        actionSerializer.Details = action.details;
                        break;
                    case SpecialAction.ActionTypeId.None:
                    default:
                        actionSerializer = null;
                        break;
                }

                if (actionSerializer != null)
                {
                    actionSerializers.Add(actionSerializer);
                }
            }
        }

        public void MapTo(BackingStore destination)
        {
            foreach(SpecialActionSerializer actionSerializer in actionSerializers)
            {
                SpecialAction tempAction = null;
                switch(actionSerializer.TypeString)
                {
                    case "Macro":
                        tempAction = new SpecialAction(actionSerializer.Name,
                            actionSerializer.Trigger, actionSerializer.TypeString,
                            actionSerializer.Details, 0, actionSerializer.Extras);
                        break;
                    case "Program":
                        tempAction = new SpecialAction(actionSerializer.Name,
                            actionSerializer.Trigger, actionSerializer.TypeString,
                            actionSerializer.Details, actionSerializer.Delay,
                            actionSerializer.Extras);
                        break;
                    case "Profile":
                        tempAction = new SpecialAction(actionSerializer.Name,
                            actionSerializer.Trigger, actionSerializer.TypeString,
                            actionSerializer.Details, 0, actionSerializer.Extras);
                        break;
                    case "Key":
                        string tempExtras = $"{actionSerializer.UnloadStyle}\n{actionSerializer.UnloadTrigger}";
                        tempAction = new SpecialAction(actionSerializer.Name,
                            actionSerializer.Trigger, actionSerializer.TypeString,
                            actionSerializer.Details, 0, tempExtras);

                        break;
                    case "DisconnectBT":
                    case "BatteryCheck":
                        tempAction = new SpecialAction(actionSerializer.Name,
                            actionSerializer.Trigger, actionSerializer.TypeString,
                            "", actionSerializer.Delay);
                        break;

                    case "MultiAction":
                        tempAction = new SpecialAction(actionSerializer.Name,
                            actionSerializer.Trigger, actionSerializer.TypeString,
                            actionSerializer.Details, actionSerializer.Delay,
                            actionSerializer.Extras);
                        break;
                    case "SASteeringWheelEmulationCalibrate":
                        tempAction = new SpecialAction(actionSerializer.Name,
                            actionSerializer.Trigger, actionSerializer.TypeString,
                            "", actionSerializer.Delay);
                        break;
                    default:
                        break;
                }

                if (tempAction != null)
                {
                    destination.actions.Add(tempAction);
                }
            }
        }
    }

    public class SpecialActionSerializer
    {
        [XmlAttribute("Name")]
        public string Name
        {
            get; set;
        } = string.Empty;

        [XmlElement("Trigger")]
        public string Trigger
        {
            get; set;
        } = string.Empty;

        [XmlElement("Type")]
        public string TypeString
        {
            get; set;
        } = string.Empty;

        [XmlElement("Details")]
        public string Details
        {
            get; set;
        } = string.Empty;

        [XmlElement("Extras")]
        public string Extras
        {
            get; set;
        } = string.Empty;
        public bool ShouldSerializeExtras()
        {
            return TypeString == "Macro" && !string.IsNullOrEmpty(Extras);
        }

        [XmlElement("Arguements")]
        public string Arguments
        {
            get; set;
        } = string.Empty;
        public bool ShouldSerializeArguments()
        {
            return TypeString == "Program" && !string.IsNullOrEmpty(Arguments);
        }

        [XmlElement("Delay")]
        public double Delay
        {
            get; set;
        }
        public bool ShouldSerializeDelay()
        {
            return TypeString == "Program";
        }

        [XmlElement("UnloadTrigger")]
        public string UnloadTrigger
        {
            get; set;
        } = string.Empty;
        public bool ShouldSerializeUnloadTrigger()
        {
            HashSet<string> knownUseTypes = new HashSet<string>()
            {
                "Key", "Profile",
            };
            return knownUseTypes.Contains(TypeString) && !string.IsNullOrEmpty(UnloadTrigger);
        }

        [XmlElement("UnloadStyle")]
        public string UnloadStyle
        {
            get; set;
        } = string.Empty;
        public bool ShouldSerializeUnloadStyle()
        {
            return TypeString == "Key" && !string.IsNullOrEmpty(UnloadStyle);
        }
    }
}
