using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Verse;
using Verse.Profile;

namespace FactionManager
{
    public static class PersistenceUtility
    {
        public static PersistenceUtilityStatus utilityStatus = PersistenceUtilityStatus.Idle;

        public static void LoadMap(string fileName, bool forced = false)
        {
            utilityStatus = PersistenceUtilityStatus.Loading;
            if(!MaxColoniesReached() || forced)
            {
                LongEventHandler.SetCurrentEventText("FM.prepareGame".Translate());
                SaveCurrentGame();
                LongEventHandler.SetCurrentEventText("FM.prepareLoad".Translate());
                PrepareSaveGame(fileName);
                LoadGameFromTmpSave(fileName);
                
                Messages.Message("FM.mapLoaded".Translate(), MessageTypeDefOf.NeutralEvent);
            }
            else
            {
                Messages.Message("CommandSettleFailReachedMaximumNumberOfBases".Translate(), MessageTypeDefOf.NeutralEvent);
            }

            utilityStatus = PersistenceUtilityStatus.Idle;
        }

        private static bool MaxColoniesReached()
        {
            return LoadedColonies(Find.World.worldObjects.Settlements.FindAll(settlement => settlement.Faction.IsPlayer)) >= Prefs.MaxNumberOfPlayerSettlements;
        }

        private static void LoadGameFromTmpSave(string fileName)
        {
            string str = GenText.ToCommaList(from mod in LoadedModManager.RunningMods
                                             select mod.ToString());
            Log.Message("Loading game with colony " + fileName + " with mods " + str);

            MemoryUtility.ClearAllMapsAndWorld();
            Current.Game = new Game();
            Scribe.loader.InitLoading(FilePathForTmpSave());
            ScribeMetaHeaderUtility.LoadGameDataHeader(ScribeMetaHeaderUtility.ScribeHeaderMode.Map, logVersionConflictWarning: true);
            if (Scribe.EnterNode("game"))
            {
                Current.Game = new Game();
                Current.Game.LoadGame();
                PermadeathModeUtility.CheckUpdatePermadeathModeUniqueNameOnGameLoad(fileName);
            }
            else
            {
                Log.Error("Could not find game XML node.");
                Scribe.ForceStop();
            }
            RemoveTmpSave();
        }

        private static void RemoveTmpSave()
        {
            string path = FilePathForTmpSave();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static void PrepareSaveGame(string fileName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(FilePathForTmpSave());
            XmlDocument mapXmlDocument = new XmlDocument();
            mapXmlDocument.Load(FilePathForSavedMap(fileName));

            XmlNode copiedNode = xmlDocument.ImportNode(mapXmlDocument.SelectSingleNode("/mapsave/li"), true);
            xmlDocument.DocumentElement["game"]["maps"].AppendChild(copiedNode);
            xmlDocument.Save(FilePathForTmpSave());
        }

        private static void SaveCurrentGame()
        {
            try
            {
                string path = FilePathForTmpSave();
                SafeSaver.Save(path, "savegame", delegate
                {
                    ScribeMetaHeaderUtility.WriteMetaHeader();
                    Game target = Current.Game;
                    Scribe_Deep.Look(ref target, "game");
                });
            }
            catch (Exception arg)
            {
                Log.Error("Exception while saving tmp game: " + arg);
            }
        }

        public static void UnloadMap(Map map, string fileName, bool forced = false)
        {
            utilityStatus = PersistenceUtilityStatus.Saving;
            if (!IsLastColony() || forced)
            {
                FixCurrentMapIfActive(map);
 
                if (SaveMap(map, fileName))
                {
                    // Changing state to not get the banished debuff as you literally don't banish your people
                    Current.ProgramState = ProgramState.Entry;
                    Current.Game.DeinitAndRemoveMap(map);
                    Current.ProgramState = ProgramState.Playing;
                    
                    Messages.Message("FM.mapUnloaded".Translate(), MessageTypeDefOf.NeutralEvent);
                }
                else
                {
                    Messages.Message("FM.mapUnloadFailed".Translate(), MessageTypeDefOf.RejectInput);
                }
            }
            else
            {
                Messages.Message("FM.errorMapUnloadLast".Translate(), MessageTypeDefOf.RejectInput);
            }

            utilityStatus = PersistenceUtilityStatus.Idle;
        }

        private static void FixCurrentMapIfActive(Map unloadedMap)
        {
            if (Current.Game.CurrentMap == unloadedMap)
            {
                var playerMaps = Find.World.worldObjects.Settlements.FindAll(settlement =>
                    settlement.Faction.IsPlayer && settlement.Map != unloadedMap);

                Current.Game.CurrentMap = playerMaps.First().Map;

                Log.Warning($"[FactionManager] CurrentMap changed to: " + playerMaps.First().Map.Parent.LabelCap);
            }
        }

        private static bool IsLastColony()
        {
            return LoadedColonies(Find.World.worldObjects.Settlements.FindAll(settlement => settlement.Faction.IsPlayer)) == 1;
        }

        private static bool SaveMap(Map map, string fileName)
        {
            try
            {
                LongEventHandler.SetCurrentEventText("Saving...");
                string path = FilePathForSavedMap(fileName);
                SafeSaver.Save(path, "mapsave", delegate
                {
                    Map target = map;
                    Scribe_Deep.Look(ref target, "li");
                });
            }
            catch (Exception arg)
            {
                Log.Error("Exception while saving map: " + arg);
                return false;
            }

            return true;
        }

        private static string FilePathForSavedMap(string fileName)
        {
            string modFilePath = Path.Combine(GenFilePaths.SaveDataFolderPath, "SavedMaps");
            CreateDirectory(modFilePath);
            string worldFilePath = Path.Combine(modFilePath, Current.Game.World.info.name);
            CreateDirectory(worldFilePath);
            string mapFilepath = Path.Combine(worldFilePath, fileName + ".rwm");

            return mapFilepath;
        }

        private static string FilePathForTmpSave()
        {
            string modFilePath = Path.Combine(GenFilePaths.SaveDataFolderPath, "SavedMaps");
            CreateDirectory(modFilePath);
            string saveFilePath = Path.Combine(modFilePath, "_tmpsave.rws");

            return saveFilePath;
        }

        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static int LoadedColonies(List<Settlement> settlements)
        {
            int count = 0;

            foreach(var settlement in settlements)
            {
                if (settlement.HasMap)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
