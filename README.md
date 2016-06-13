## MouseChef

This is a program which reports some nice stats about mice.

The idea is that you take two mice, tape them together like a mad
scientist, and move them around as a unit.

![two mice taped together](https://github.com/rspeele/MouseChef/raw/master/2mice.jpg "I'm serious.")

Since they're recording the same physical motions, you can safely
compare the events they reported to the operating system to know the
exact DPI ratio between the two mice.

Example:

* Mouse A moved 5 units on the x axis and 10 units on the y axis.
* In the same time period, mouse B reported 20 units on the x axis and 40 units on the y axis.
* Therefore, mouse B's DPI is 4 times higher than mouse A's.

You can also use this technique to detect input lag or whether a mouse
has built-in acceleration (compared to another mouse).

### How to use it

First, tape your mice together. It doesn't have to be a rock solid
connection. As long as they move together without slipping around
relative to one another, you should be fine. Painter's tape is good
for not leaving too much sticky residue.

Next, start up MouseChef and hit ctrl+space to start recording input.
Move the two mice around. Try slow movements, fast movements, both
axes, stopping and starting, whatever you can think of.

Whatever you do, *don't* rotate the taped together mice. That will
ruin everything, because unless the center of rotation is exactly at
the midpoint of the two sensors, they'll move different distances. In
the worst case scenario, if you rotate perfectly around the sensor of
one mouse, it won't pick up any movement at all while the other mouse
will pick up a lot of movement.

The following diagram demonstrates this problem:

![example visualization of rotation problem](https://github.com/rspeele/MouseChef/raw/master/badrot.png "VERY BAD.")

Once you've recorded enough input, just hit ctrl+space again to stop
recording. Now you can play around with the UI.

![UI cheatsheet](https://github.com/rspeele/MouseChef/raw/master/cheatsheet.png)