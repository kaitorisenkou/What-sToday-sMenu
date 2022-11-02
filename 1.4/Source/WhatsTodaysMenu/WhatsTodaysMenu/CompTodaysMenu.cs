using RimWorld;
using Verse;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WhatsTodaysMenu {
    public class CompTodaysMenu : ThingComp {
        CompIngredients compIngredients;
        static IEnumerable<MenuPatternDef> defsSort;
        public override string TransformLabel(string label) {
            if (compIngredients == null) {
                compIngredients = parent.TryGetComp<CompIngredients>();
            }
            if (compIngredients != null) {
                return GetMenuName();
            }
            return base.TransformLabel(label);
        }
        public override string CompInspectStringExtra() {
            return parent.def.label;
        }
        public override bool AllowStackWith(Thing other) {
            bool result = false;
            if (compIngredients != null) {
                var otherIng = other.TryGetComp<CompIngredients>();
                if (otherIng != null) {
                    result = otherIng.ingredients.All(t => compIngredients.ingredients.Contains(t));
                } else {
                    return false;
                }
            } else {
                result = true;
            }
            return result && base.AllowStackWith(other);
        }


        string menuName;
        public string GetMenuName() {
            if (menuName == null) {
                var props = (CompProperties_TodaysMenu)this.props;
                var rank = props.mealRank;
                if (defsSort == null) {
                    defsSort = DefDatabase<MenuPatternDef>.AllDefs.OrderByDescending(t => t.priority);
                }
                if (defsSort != null && defsSort.Any()) {
                    var ingredientsInComp = compIngredients.ingredients;
                    foreach (var i in defsSort) {
                        if (i.mealRank != rank) continue;
                        if (i.mainIngredient.IngredientContain(ingredientsInComp) != null) {
                            if (i.ingredients == null || !i.ingredients.Any() || i.ingredients.All(t => t.IngredientContain(ingredientsInComp) != null)) {
                                menuName = i.mainIngredient.GetMenuLabel(i.label, ingredientsInComp);
                                break;
                            }
                        }
                    }
                }
            }
            return menuName != null ? menuName : parent.def.label;
        }
    }
}
