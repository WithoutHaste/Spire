Text Flow
=========

Basics
------

Line break between words

Line break between syllables

Multiple columns of text

Left justify

Right justify

Center justify

Left/Right justify

---

Spell Check
===========

Basic English

Specialized English: medical, chemistry, engineering, mathematical...

---

Grammar Check
=============

Basic English

---

Page
====

Support wide range of page sizes and margins. Any size really.

Support absolute margins (where nothing is printed) and available margins (where main text does not go, but sidenotes and images can).

Let user label page layouts and apply them to all pages, or to select pages. Let user say "use same page layout on this page as on this other page".

Page Layout Mode
----------------

Show just blue boxes anywhere text flow goes, with arrows from the end of one to the beginning of the next, and a final arrow from the last one to "Next Page" label.

---

Menus
=====

Design menu/button setup such that I can try out many designs easily, and even let users switch between designs.

Favor vertical side panels - monitors are wide, lots of real estate there.

Use all text labels instead of image icons. Icons add to the learning curve of any program, skip that with text.

---

Styles
=======

Do not auto-style (ex: assuming the next line will be part of the previous list)

CSS-like styling with variables.

Separate content from structure.

	Named styles like "Title" and "Abstract" both identify important parts of the document, and determine how it should be displayed.
	
	This will also make it possible to export the document into different publisher's formats.

Support bumping all headers up or down one order of magnitude. Makes it much easier when you need to add a new level.
	
---

Exporting
=========

Export to PDF required. Standard sharing format.

Export to DocX required. Verified required by some publishers.

Export to TXT required. Always allow text, basic principle of programming.

Export formulas as images.

Export just graphics.

---

Importing
=========

Import from plain text.

Optionally convert formulas.

Optionally convert styles.

Will probably want to import from Markdown, and similar text formats, too.

Will probably need import from DocX, but not first thing.

---

Data Graphics
=============

Process
-------

	* Load the data table (like from Excel)
	* Convert data table to internal format
	* Pass data to graphic plug-in
	* The plug-in returns an image
	* Save the image with the document
	
Make it easy to see several graph-types of one data set so you can choose the correct graph-type.

	Plugins will need a standard for assigning columns to axis to make this possible.
		Easy for line graph and scatter plot: x and y axis are clear
		Will need a standard for the maps to follow

	If showing a full screen of options, I prefer paging over scolling, so one mouse click brings an entirely new page of options up.
	
Types of Graphs
---------------

	Scatterplot
		and bivariate scatter
		axis option dash at each data point, label high and low
		axis option highlight a single data point with an arrow to it along each axis
		data point option show label instead of point
		data point option show image instead of point
		Phillip's Curve - connect the dot by a third variable
	Histogram
	Line Chart
		option to highlight High, Low, and Average
	Stem and Leaf
	Cosmograph
	Geographical Maps
		I'd like to include a This Spaceship Earth by Countries
		At least have USA by States
		? USA by States with equal shape/size states set in approximate correct positions ?
			Can I make a generic process to translate geographical regions like that?
	Heat Map
	Tree Graph
	Data Words
		sparklines and dashlines

No pie charts in core set. Others can make them if they want.

Multi-graph Graphs
------------------

Stacked Graphs (vertical or horizontal)

Small Multiples

Overlaid Graphs

Plugins
-------

Do they need to indicate a range of data quantities they are appropriate for?

Do they need to indicate how small an image they can produce legibly?

Should they return a mapping of blank space in the graph that text can flow over?

Graphs will need to default to greyscale and accept a color-scheme. Transparent backgrounds required.

Plugin naming convention is TYPE.SPECIFICS[.SPECIFICS].UNIQUE
	
	PieChart.3D.Snow, PieChart.3D.Platypus, PieChart.2D.Andromeda
	Map.USA.States.BlueJay, Map.USA.Montana.Districts.Davis
	
Any use for a utility that inflation adjusts US dollar amounts by year?
	
Save full data table with document, or just reference it?
---------------------------------------------------------

__Concerns:__
	
	* You may want your graphic to auto-update when the data changes
	* You may not want to share the raw data with whoever you share the document with
	* You may _want_ to share the raw data with the document

__Conclusion:__
	
Leave this up the user on an individual data graphic basis.

Let user create small data tables within Spire?
-----------------------------------------------

__Concerns:__

If you just need to enter a few lines of data, should you have to open excel or notepad to do it, and then import that file into Spire?

Well, Spire is not trying to be excel. This sounds like a feature that would creep larger.

__Conclusion:__

All data tables must be imported, even little ones.

Tufte Principles
----------------

"the number of variable dimensions should not exceed the number of dimensions in the data"
	
People are generally bad at determining the volumes of circles, or areas of different shapes.

---

Formulas
========

Principles
----------

	* Pure text, mouse not needed
	* Convert to symbols as formula is typed - quickly legible
	* Formulas can be written in-line with document - treated as normal text

Format
------

Ex: ##integral(0 inf x^2 - 1)##

__##__ to start and end
	
Configuration
-------------

Set start and end characters.

	Ex: ## and ##
	Ex: ##C and ## for Calculus, and ##L and ## for Logic

Set multiple formats that convert to the same symbol or layout.

	Ex: choose(A,B) or (A choose B)
	Ex: inf for infinity sign, pi for pi sign
	
Let users edit/create any configuration they want, just validating there are no collisions.  Everything will be saved to an internal structure, so users can even share with different configurations.

	* The best configurations will be shared and become standard.
	* Everyone can satisfy their idiosyncrasies.

Silo-ing
--------

Use multiple configuration files at once. Maybe one for Calculus, one for Logic. Separate any that will not be used together in one formula.

Differentiate the configurations with the start characters.

	Ex: ##C and ## for Calculus, and ##L and ## for Logic

---

Footnotes
=========

Support footnotes tied to sentences or words in the text.

Two options:

	A) Show footnotes at bottom of page footnote appears on
	B) Show footnotes in an appendix

What page does the footnote go on?
----------------------------------

__Concerns:__

Lia brought up that the standard seems to be that the footnote appears on the page where the beginning of the sentence occurs, even if the footnote symbol occurs at the end of the sentence on the next page.

This means the reader has to flip back to read the footnote.

I agree that this is annoying.

__Is there actually a standard?__

Purdue Online Writing Lab says "place all footnotes at the bottom of the page on which they appear" which does not clear up this issue.

__Conclusion:__

If there is no standard, place the footnote on the page where the footnote _symbol_ appears.

If there is a standard, make this an option.

---

Sidenotes
=========

Like footnotes, but occurs on the side of the page. Many textbooks and reference books use these well.

Tie location of sidenote to a point in the text. Attempt to place sidenote near reference point.

---

Table of Contents
=================

Support automatic table of contents.

---

Bibliography
============

Import bibliography from reference managers (ask Kathy for more details).

Display bibliography in various formats. Include configuration options so new formats can be entered or downloaded by user.

---

Languages
=========

To make it easier to add new languages later, make a phrase dictionary. For any phrase visible to the user, pull it from a configuration file.

	Ex: English.Dictionary.MenuConfiguration = "Configure"
	Ex: English.Dictionary.NounNextPage = "Next Page"
	
Make a Languages folder to contain these configs, one per language. Can test with Leet vs Engligh since I don't know another language.

Is there a standard format for this kind of file?

Include small C# program MergeLanguageFiles that will load the english master file and another language file of your choice, and add any lines from english that are missing from the other with "??" as the translation. This will also sort the other file to match the english file order. This is so you can easily pick up changes from the english file.

---

Data Annotations
================

Allow simple overlays of text and arrows over any graphic, for explanations, bringing part of it to the attention of the viewer, etc.

Allow user to set margin sizes (all four sides) for any graphic, and let the overlay use the margin space as well.

	Ex: (VD of QI pg 68) highlighting a range of data and showing % change

---

Appendices
==========

Automatic appendix of tables with title, location.

Automatic appendix of figures with title, location.

Automatic appendix of graphs with title, location.

Let user define what is a table, figure, graph on each (with default settings).