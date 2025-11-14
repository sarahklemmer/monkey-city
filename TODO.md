QOL code changes (out of date):
1. *DONE* make it so that buildings become invisible in "placement" mode so you can place buildings next to each other
2. *DONE* make raycast based movement detection in monkeySelector only hit the ground using layer masks rather than buildings
3. make far more robust selection system with the selector being a templated data structure that accepts a "Selectable" object that implements an OnSelect() and OnDeselect() function. for goldspike because it's only two things im just duplicating the code
4. make it so that child classes of BuildigBase don't all have to call base.SharedAwakeBehavior
5. make it so that the chimps stay level rather than rising in the air when attacking tall buildings
6. make it so that all classes extending buildingbase are forced to specify their capacity
7. *DONE* when a building is destroyed, the monkeys inside it come out
8. isloate changing allocation state better in monkeycontroller and buildingmonkeys, as right now they interfere with each other and that shouldn't be the case

UX/UI:
*DONE* take out tree of life from building menu
*DONE* Change camera rotating from snapping to a spin
*DONE* instantly regenerate building health after wave 
*DONE* fix rendering bug
*DONE* let players place buildings wherever in tutorial
*DONE* update the icons for buildings
*DONE* shift click for building info
*DONE* monkeys can no longer move on the map
*DONE* change dollar sign in menu to bananas
*DONE* building menu flashing when new building is unlocked, building unlock system
*DONE* make placement 2X2 instead of 1X1
*DONE* display resource generation numbers
*DONE* make monkeys walk faster


indicators instead of text for tutorial
make it visually clear without clicking that monkeys are in buildings
keybinds to quick assign monkeys to nearest banana farm or archer tower
icons or some kind of feedback for when buildings are regenerating or generating bananas
make tutorial directly railroady instead of text
indicate to the player when chimps are coming 
make it clearer exactly how many chimps your archer towers will kill

Bugs:
limit monkeys and move around monkeys

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
