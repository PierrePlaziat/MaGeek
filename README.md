![MaGeek](/Graph/Title.png "MaGeek")

# MaGeek

WPF MTG Library Manager 

## Description

I wanted a tool to manage my MTG card collection on local drive, that reflect my needs, which can eventually evolve.
Aimed toward paper MTG game, so online only cards are not included.
Import all cards at first run (fun cards included), or start fresh and import cards at your will, 
but initial importation is criticaly faster.

## Functionalities

-	All paper cards support including fun
-	Decks/sets/lists gestion
-	Proxy print
-	Foreign language suport
-	Auto update card data
-	IDE style movable panels
-	Stats and tags

#### Known issues

-	"stitch in time" has 2 archetypes for its 3 variants
-	sometimes no names in deck content
-	biface cards traductions not present
-	some preco imports can give error
-	restart app only stops it
-	ui async needs polish (ui sometime blocks)
-	need to reselect card after tag add or card update
-	wont update gotCounts in card searcher until new search
-	original layout disposition horizontal sizes blocked (because trick to maxsize card inspector)

### Ideas

-	bulk import later than first launch (you can currently simply delete the db to force first launch)
-	better print experience (prevent basic land to print, only one window, get rid of the wpf win clunchy teknik)
-	Custom cards print support (need to figure out how to generate the image for print (maybe Markdown??)
-	Auto deck (need to reflect on the building strategy)
-	Tags sort and stats for deck (need refactoring some tools)
-	Pie charts for stats (could be cool but is not needed)
-	better double sided gestion (only data from front card are stored (except the image, that contains all infos)
-	differentes variant in deck (EF concerns)
-	Avalonia port (if ever need multiplatform)
