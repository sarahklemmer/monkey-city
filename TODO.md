1. *DONE* make it so that buildings become invisible in "placement" mode so you can place buildings next to each other
2. *DONE* make raycast based movement detection in monkeySelector only hit the ground using layer masks rather than buildings
3. make far more robust selection system with the selector being a templated data structure that accepts a "Selectable" object that implements an OnSelect() and OnDeselect() function. for goldspike because it's only two things im just duplicating the code
4. make it so that child classes of BuildigBase don't all have to call base.SharedAwakeBehavior
5. make it so that the chimps stay level rather than rising in the air when attacking tall buildings
6. make it so that all classes extending buildingbase are forced to specify their capacity
7. *DONE* when a building is destroyed, the monkeys inside it come out
8. isloate changing allocation state better in monkeycontroller and buildingmonkeys, as right now they interfere with each other and that shouldn't be the case
9. have indiciators that regeneration is possible/happening

Todo before playtest:
<!-- move sun for lighting issue -->
<!-- shouldn't be able to visually select monkey during tutorial -->
<!-- shrink toast  -->
<!-- make repeat instruction button say repeat instruction -->
<!-- close the building menu when you select a building -->
<!-- when you open the building menu get rid of indicators if present -->
repeat instruction button repeats too many times
<!-- make toast background black when building info is selected -->
<!-- able to click banana farm in tutorial after both monkeys allocated -->
close building menu by clicking off
make building icons their models
<!-- raise y level of archer tower -->
<!-- get rid of remove button deleting archer tower -->
<!-- move archer tower closer to center -->
<!-- fix tree of life tooltip should say 2 days instead of 3 days -->
<!-- make production number accurate -->
<!-- X to get out of buildinginfo -->
<!-- click through toasts -->

UX/UI:
make monkeys walk faster
Change camera rotating from snapping to a spin
indicators instead of text for tutorial
fix rendering bug
take out tree of life from building menu
let players place buildings wherever in tutorial
update the icons for buildings
make it visually clear without clicking that monkeys are in buildings
keybinds to quick assign monkeys to nearest banana farm or archer tower
shift click for building info
monkeys can no longer move on the map
icons or some kind of feedback for when buildings are regenerating or generating bananas
change dollar sign in menu to bananas
instantly regenerate building health after wave 
make tutorial directly railroady instead of text
indicate to the player when chimps are coming 
make it clearer exactly how many chimps your archer towers will kill
display resource generation numbers

Bugs:
monkeys getting stuck next to buildings
weird rendering bug (i know what this bug is i can fix it -elliot)
limit monkeys and move around monkeys \

Gameplay:
3-4 upgrades per building (each visually different)
make building upgrade more attainable
fix difficulty curve
move buildings
limit buildings and monkeys
path system with 3 paths and 3 buildings
fix walls
attack every 7 days and specify how many chimps are coming, ui element
buildings have an upkeep 
limit monkeys

chimps attack from set locations or attack from set locations most of the time
