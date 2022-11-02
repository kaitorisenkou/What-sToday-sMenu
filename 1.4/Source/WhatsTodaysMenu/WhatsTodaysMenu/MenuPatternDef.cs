using Verse;
using System.Collections.Generic;

namespace WhatsTodaysMenu {
    public class MenuPatternDef : Def {
        public int priority = 0;
        public IngredientCategoryDef mainIngredient;
        public List<IngredientCategoryDef> ingredients;
        public string mealRank;
    }
}
