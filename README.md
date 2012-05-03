monotouch-element-pack
======================

A place to host various MonoTouch.Dialog.Elements that users can use.

RowBadgeElement
===============

Shows a row with a colored badge

SimpleMultilineEntryElement 
===========================

A simple element that provides multi-line editing

CounterElement
==============

The CounterElement is used to get user input for decimal values and is
intialized with two string values: the caption for the entry and the
current value. Both values are required. The initial numeric value may
be the empty string.

When the CounterElement is selected, the numeric value is displayed
with a picker providing a wheel for each of the digit positions. An
additional wheel is shown at the left, allowing the number to grow
with one digit position. This leftmost wheel has a blank instead of
a 0.

So 12.50 will be shown with as a picker with 5 wheels having values
blank, 1, 2, 5 and 0. When e.g. 6 is selected with the leftmost wheel,
the value becomes 612.50 and subsequent selects will show a picker
with 6 wheels having values blank, 6, 1, 2, 5 and 0. If now the 6 and
1 are set to zero, the value becomes 2.50 and subsequent selects will
show a picker with 4 wheels with values blank, 2, 5 and 0.

The decimal wheels have a lightgrey background. Decimal wheels will
only be present if the input value contains a decimal point (, or .)
and the number of decimal wheels is fixed as indicated by the initial
value.

This element was contributed by Guido Van Hoecke.
