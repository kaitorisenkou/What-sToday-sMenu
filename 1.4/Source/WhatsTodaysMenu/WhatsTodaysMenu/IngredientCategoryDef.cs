using Verse;
using System.Linq;
using System.Collections.Generic;

namespace WhatsTodaysMenu {
    public class IngredientCategoryDef : Def {
        public List<string> ingredientsPartialMatch;
        public List<ReplaceProperties> replaceProperties;
        public string duplicatedLabel = null;

        public ThingDef IngredientContain(List<ThingDef> ingredients) {
            foreach (var i in ingredientsPartialMatch) {
                var a = ingredients.Where(t => t.defName.Contains(i));
                if (a.Any()) return a.First();
            }
            return null;
        }
        public int IngredientContainCount(List<ThingDef> ingredients) {
            foreach (var i in ingredientsPartialMatch) {
                var a = ingredients.Where(t => t.defName.Contains(i));
                if (a.Any()) return a.Count();
            }
            return 0;
        }
        public string GetMenuLabel(string baseName, List<ThingDef> ingredients) {
            string name = baseName;
            if (duplicatedLabel != null && IngredientContainCount(ingredients) > 1) {
                return name.Replace("/ingredient", duplicatedLabel);
            } else {
                name = name.Replace("/ingredient", IngredientContain(ingredients).label);
            }
            
            if (replaceProperties != null && replaceProperties.Any()) {
                foreach (var i in replaceProperties) {
                    name = name.Replace(i.wordPreReplace.Translate(), i.wordPostReplace.Translate());

                }
            }

            return name;
        }
    }

    public class ReplaceProperties {
        public string wordPreReplace = null;
        public string wordPostReplace = null;
    }
}
