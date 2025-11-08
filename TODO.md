1. *DONE* make it so that buildings become invisible in "placement" mode so you can place buildings next to each other
2. *DONE* make raycast based movement detection in monkeySelector only hit the ground using layer masks rather than buildings
3. make far more robust selection system with the selector being a templated data structure that accepts a "Selectable" object that implements an OnSelect() and OnDeselect() function. for goldspike because it's only two things im just duplicating the code
4. make it so that child classes of BuildigBase don't all have to call base.SharedAwakeBehavior
5. make it so that the chimps stay level rather than rising in the air when attacking tall buildings