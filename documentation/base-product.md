Base Product: Notepad-level Word Processor
Milestone set 1
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

2b. Set up expansion/collapse of chunks in DocumentModel.

	Test adding text randomly, a lot.

3. Delete Key and Backspace Key.
	
	Load Test: add 100,000 characters: 2-3 seconds
	Load Test: delete 100,000 characters: 2-8 seconds
	
	Test adding and removing text randomly, a lot.
	Test backspace correctly.
	Test backspace at beginning of text.
	Test delete correctly.
	Test delete at end of text.

--> current unit tests
	
4. Line wrap on characters.

	Test display no text.
	Test typing along one line.
	Test navigating past beginning of text.
	Test navigating past end of text.
	Test typing onto second line.
	Test navigating back to first line.
	Test navigating back to second line.
	Test typing onto third line.
	Test typing text back into the first line after several lines are in.

4b. Line wrap on whole words.

--> current developement

4c. Make running unit tests part of standard build process.

4d. Make test app for all combinations of Graphics display options.

	Find the one(s) that fix that 1 pixel wiggle on tall letters.	
	
5. Up, Down Arrow Keys to navigate edit position.

5b. Click with mouse to navigate edit position.

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

22. Break words with hyphen if word is longer than line.

