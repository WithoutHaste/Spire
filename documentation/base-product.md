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

4. Line wrap on characters.

	All tests overridden by 4b test.

4b. Line wrap on whole words.

	Test display no text.
	Test typing along one line.
	Test typing onto second line.
	Test typing onto third line.
	Test navigating back to first line and typing more.
	Test navigating back to second line and typing more.
	Test typing words almost too long for one line.
	Test typing words too long for one line.
	Test removing text from various parts.

--> current developement

4c. Make test app for all combinations of Graphics display options.

	Find the one(s) that fix that 1 pixel wiggle on tall letters.	
	
5. Up, Down Arrow Keys to navigate edit position.
	
	Test moving up a line
	Test moving down a line
	Visually test moving down when lower line is much shorter
	Visually test moving up at beginning of line
	Visually test moving up at first character of line
	Visually test moving down at beginning of line
	Visually test moving down at first character of line

5b. When removing characters from first word in line, check if the word now fits on the previous line.

	Test delete characters from long word until it ought to fit back on previous line

5c. Fix bug: when second line is longer than first, up arrow from second line lands you back in the second line.

	Test move up from longer line to shorter, from point after end of shorter line, should end up at end of shorter line.

6. Undo.

	empty file, undo
	type one letter, undo (all)
	type several letters, undo (all)
	type 100 letters, undo (some)
	type several words, undo (one word) undo again (another word)
xx	type part of word, wait awhile, type more words, undo
	type two words, delete/backspace the space in between, undo (undoes the delete)
	type hyphenated words, undo
	type two words, replace space with hyphen, undo
	type a long words, put a hyphen in the middle, undo, undo
	type word with comma at end, undo
	type word with period at end, undo
	test other punctuation
	type word, move cursor, go back to end of word, type more, undo
	type word, type in another part of document, add to first word with no space, undo, undo, undo
	type word, move cursor, go to middle of word, type more, undo, undo
	test numbers count as characters
	test delete and backspace

7. Redo.

	test redo on new document
	test redo typing
	test redo backspace
	test redo delete
	test redo past end of redo list
	test several undos then several redos
	test that editing clears redo list

7b. Separate long running tests from fast tests.

--> current unit tests

8. Click with mouse to navigate edit position.

--> current Linux developement

9. Shift + Arrow Keys to highlight text.

9b. Drag and Drop to highlight text.

9c. Delete highlighted text.

9d. Backspace hightlighted text.

9e. Type over highlighted text.

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

