using UnityEngine;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;

namespace WhatsTodaysMenu {
    [StaticConstructorOnStartup]
    public class WhatsTodaysMenu {
        /*
        public static string[] menuName;
        public static string[] menuRank;
        public static string[][] menuIngredients;
        */
        static WhatsTodaysMenu() {
            Log.Message("[WhatsTodaysMenu] Now Active");

            var patterns = MenuPatternDefOf.menuPatterns.menuPattern;
            List<string> menuNameList = new List<string>();
            List<string> menuRankList = new List<string>();
            List<string[]> menuIngredientsList = new List<string[]>();
            foreach (var i in patterns) {
                var spl = i.Split(',');
                menuNameList.Add(spl[0]);
                menuRankList.Add(spl[1]);
                menuIngredientsList.Add(spl.Skip(2).ToArray());
            }
            LabelPatch.menuName = menuNameList.ToArray();
            LabelPatch.menuRank = menuRankList.ToArray();
            LabelPatch.menuIngredients = menuIngredientsList.ToArray();

            Log.Message("[WhatsTodaysMenu] Load Complete");
            var harmony = new Harmony("kaitorisenkou.WhatsTodaysMenu");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    /*
    //[HarmonyPatch(typeof(CompIngredients),MethodType.Constructor)]
    [HarmonyPatch(typeof(CompIngredients))]
    [HarmonyPatch(nameof(CompIngredients.CompInspectStringExtra))]
    public static class InspectPatch {
        public static string[] menuName;
        public static string[] menuRank;
        public static string[][] menuIngredients;

        static void Postfix(CompIngredients __instance, ref string __result) {
            var ingredients = __instance.ingredients.GroupBy(t => t.ToString()).Select(t => t.First()).OrderBy(t => t.ToString());
            var ingredientsString = ingredients.Select(t => t.ToString());
            var ingredientsCount = ingredients.Count();

            var meat = ingredients.Where(t => t.ToString().Contains("Meat"));
            int meatCount = meat.Count();
            var coockingRank = __instance.parent.ToString();

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < menuName.Length; i++) {
                if (coockingRank.Contains(menuRank[i])) {
                    bool pass = true;
                    var ingStrList = new List<string>(ingredientsString);
                    foreach (var y in menuIngredients[i]) {
                        if (ingStrList.Any(t => t.Contains(y))) {
                            ingStrList.Remove(ingStrList.First(t => t.Contains(y)));
                        } else {
                            pass = false;
                            break;
                        }
                    }
                    if (pass) {
                        result.Append(menuName[i].Translate(
                            meatCount > 1 ? 
                            (string)"groundMeat".Translate()
                            : 
                            (meatCount > 0 ? meat.First().label.Replace("meatPreReplaced".Translate(), "meatPostReplaced".Translate()) : "")
                            ));
                        result[0] = char.ToUpper(result[0]);
                        break;
                    }
                }
            }
            if (result.Length>0) {
                result.Append('\n');
                __result = result.Append(__result).ToString();
            }
        }
    }
    */

    [HarmonyPatch(typeof(ThingWithComps))]
    [HarmonyPatch(nameof(ThingWithComps.LabelNoCount), MethodType.Getter)]
    public static class LabelPatch {
        public static string[] menuName;
        public static string[] menuRank;
        public static string[][] menuIngredients;

        static void Postfix(ThingWithComps __instance, ref string __result) {
            var comp = __instance.GetComp<CompIngredients>();
            if (comp != null) {
                var ingredients = comp.ingredients.GroupBy(t => t.ToString()).Select(t => t.First()).OrderBy(t => t.ToString());
                var ingredientsString = ingredients.Select(t => t.ToString());
                var ingredientsCount = ingredients.Count();

                var meat = ingredients.Where(t => t.ToString().Contains("Meat"));
                int meatCount = meat.Count();
                var coockingRank = comp.parent.ToString();

                StringBuilder result = new StringBuilder();
                for (int i = 0; i < menuName.Length; i++) {
                    if (coockingRank.Contains(menuRank[i])) {
                        bool pass = true;
                        var ingStrList = new List<string>(ingredientsString);
                        foreach (var y in menuIngredients[i]) {
                            if (ingStrList.Any(t => t.Contains(y))) {
                                ingStrList.Remove(ingStrList.First(t => t.Contains(y)));
                            } else {
                                pass = false;
                                break;
                            }
                        }
                        if (pass) {
                            result.Append(menuName[i].Translate(
                                meatCount > 1 ?
                                (string)"groundMeat".Translate()
                                :
                                (meatCount > 0 ? meat.First().label.Replace("meatPreReplaced".Translate(), "meatPostReplaced".Translate()) : "")
                                ));
                            result[0] = char.ToLower(result[0]);
                            break;
                        }
                    }
                }
                if (result.Length > 0) {
                    result.Append('(');
                    result.Append(__result);
                    result.Append(')');
                    __result = result.ToString();
                }
            }
        }
    }

    public class MenuPatternDef : Def {
        public List<string> menuPattern;
    }

    [DefOf]
    public static class MenuPatternDefOf {
        public static MenuPatternDef menuPatterns;
        static MenuPatternDefOf() {
            DefOfHelper.EnsureInitializedInCtor(typeof(MenuPatternDefOf));
        }
    }
}
