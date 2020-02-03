# Node-State-Machine

Maszyna stanów na nodach (taka jak Animator, ale nie potrzeba animacji) do łatwiejszego zarządzania projektem. Idealna do robienia sztucznej inteligencji lub kontrolera dla gracza. Własne akcje robi się dziedzicząc po StateMachine.Action a własne warunki dziedzicząc StateMachine.Condition (patrz przykłady).

To Do List
1. Dodać selead do klas, po których nie powinno się dziedziczyć.
2. Napisać InputManager do przykładu.
3. a) State powinien dziedziczyć po ScriptableObject i zapisywać się w StateMachine przy tworzeniu
   b) dzięki temu można usunąć OnStateSelected i zamiast tego użyć Selection.SetActiveObjectWithContext(statemachine, state)
4. Zmienić edytor dla StateMachine żeby nie trzeba było tworzyć akcji ręcznie i ich przeciągać tylko żeby była lista akcji do wyboru i po wybraniu żeby się tworzyła i zapisywała w StateMachine
5. To samo co wyżej tylko dla warunków
6. Zrobić czasowy State, który jak śie skończy przechodzi do innego
7. Żeby dało się tworzyć zmienne jak w Animatorze i odwoływać się do nich w akcjach i warunkach
8. Ąkcje żeby były wyświetlane w formie ReordableList
9. To samo co wyżej dla warunków
