This project is from a GameJam organized by alumni of TecnoCampus, with some teachers as a jury. The theme proposed was "75%" (for whatever reason).

It's a simple game, with simple goals and mechanics. But I managed to be very interested in the task I was assigned for it, the liquid simulating and core mechanic.
I decided to use celular automata to try and find the most realistic and less resource consuming result, although it's a small minigame I used the opportunity to learn by doing.

The goal of this minigame is to press and hold a button to pour coffee onto different kinds of cups, then let go the button to stop right at 75% of that cup.

First, using black and white masks I managed to set irregular boundries on a square grid so we could implement different containers easily:

![image](https://github.com/user-attachments/assets/16d483fb-e34f-4566-ad7e-4378b15e0008)
![image](https://github.com/user-attachments/assets/ce60cf0c-4c29-4275-a946-5fcbdae5aaee)
![image](https://github.com/user-attachments/assets/8b197604-238b-4e30-849e-2c6f6e0e1781)

Then it was time to set my celular automata's rules, using two arrays; the main grid and a grid mask that sets the boundries for the liquid.

Each cell of the main grid can be either static (out of boundry), occupied/filled or foam. 

This foam cell is what makes the core mechanic interesting, since it will raise while coffee is pouring but drop once it stops.

![PouringCup](https://github.com/user-attachments/assets/89894b03-1836-4a29-9101-95576113a19a)

Once the liquid's behaivour is set, I played adding milk with another input and checking how much milk over coffe is there:

![PouringMilk](https://github.com/user-attachments/assets/94102356-7e56-4aad-99bd-de32407f85be)
![image](https://github.com/user-attachments/assets/844c149a-4ba7-4e6b-bfb2-de65734f5b22)

It was a quick GameJam and I had not much disponibility at the time, but it was fun! Check the game at my itch.io: https://franysan.itch.io/
