Base Product: Notepad-level Word Processor
==========================================

What automated unit tests make sense?

What load testing makes sense?

1. Type. See text appear on screen, with indicator of position.

	Test all characters on keyboard except Tab and Enter.
	Test capslock.
	Test shift plus characters.
	Test caret on new line.
	Test caret during typeing.
	Test caret after one or several spaces.

2. Left, Right Arrow Keys to navigate edit position.

	Test caret position as cursor moves along line.
	Test going left.
	Test going right.
	Test entering text within line.

--> current unit tests
	
2b. Set up expansion/collapse of chunks in DocumentModel.

	Test adding text randomly, a lot.

--> current developement

3. Delete Key and Backspace Key.

4. Line wrap on whole words.

5. Up, Down Arrow Keys to navigate edit position.

6. Undo.

7. Redo.

8. Shift + Arrow Keys to highlight text.

9. Drag and Drop to highlight text.

10. Home Key and End Key.

11. Save, Save As, Load, New.

12. Enter Key

13. Tab Key

14. Copy, Paste, Cut.

	Test copy from other programs.
	Test copy to other programs.

15. Show page breaks.

16. Page Up, Page Down.

17. Control + Arrow Keys to move by word chunks.

18. Control + Delete or Backspace to remove by word chunks.

	Always stop at punctuation, any punctuation.
	
19. Find.

20. Replace.

21. Print.