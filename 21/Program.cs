using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace _21
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");

            var foods = input.Split("\r\n").Select(l =>
            {
                var m = Regex.Match(l, @"^([\w\s]+)*(\(contains\s[\w,\s]*\))*$");
                var ingredients = m.Groups[1].Value.Trim().Split(" ");
                var alergens = Enumerable.Empty<string>();
                if (m.Groups.Count > 2)
                {
                    var alg = m.Groups[2].Value.Trim().Replace("(contains", "").Replace(")", "");
                    alergens = alg.Split(",").Select(a => a.Trim());
                }
                return (ingredients, alergens);
            });

            var ingredients = foods.SelectMany(food => food.ingredients).Distinct();
            var alergens = foods.SelectMany(food => food.alergens).Distinct();

            var alergensToFood = alergens
                .Select((alergen, alergen_ix) =>
                    (ix: alergen_ix, food: foods.Select((f, ix) => (f, ix))
                        .Where(food_ix => food_ix.f.alergens.Contains(alergen))
                        .Select(food_ix => food_ix.ix))
                )
                .OrderByDescending(algtorec => algtorec.food.Count());

            var alergenIngredientsCandidates = alergensToFood.Select((alergenFood, alfIx) =>
                (ix: alfIx,
                ingredients:
                    foods.Where((_, ix) => alergenFood.food.Contains(ix))
                        .Select(f => f.ingredients.AsEnumerable())
                        .Aggregate((f1, f2) => f1.Intersect(f2))
                )
            ).OrderBy(ac => ac.ingredients.Count());

            var foundAlergens = new List<string>();
            var foundAlergensIngredients = new List<string>();

            while (foundAlergens.Count() != alergens.Count())
            {
                foreach (var alergenIngredientCandidate in alergenIngredientsCandidates)
                {
                    if (alergenIngredientCandidate.ingredients.Count(ing => !foundAlergensIngredients.Contains(ing)) == 1)
                    {
                        foundAlergens.Add(alergens.Skip(alergenIngredientCandidate.ix).First());
                        foundAlergensIngredients.Add(alergenIngredientCandidate.ingredients.First(ing => !foundAlergensIngredients.Contains(ing)));
                    }
                }
            }

            var nonAlergenIngredientsCount = foods.Aggregate(0, (sum, food) => sum += food.ingredients.Count(i => !foundAlergensIngredients.Contains(i)));

            Console.WriteLine($"{nonAlergenIngredientsCount}");
            Console.WriteLine($"{string.Join(',', foundAlergens.Select((alg, ix) => (ix, alg)).OrderBy(algix => algix.alg).Select(algix => foundAlergensIngredients[algix.ix]))}");
        }
    }
}
