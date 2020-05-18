<h3>Flights Center</h3>
<h4>A Full-stack flights application</h4>

(This project was built and tested in .NET Framework 4.6.1)

<b>Please notice:</b> this application is a demo, and created for educational porpuses only, and not for use in the real world.

---------------------

<b>Flights Center</b> is a website that allows customers all around the world to buy tickets to commercial flights. It also allows airline companies to create accounts, add and manage their flights in the website.

The website contains a flights search page, a deals page, a flights board and private pages for each user.

These are the project layers and the technologies that was used:

![Project Layers](https://user-images.githubusercontent.com/45973605/82200357-8c8b9480-9907-11ea-8746-92fec80896b7.jpg)
Features list:

<ul>
  <li><b>Design patterns:</b> There are two design patterns that is used in this project: Facade and Singleton. </li>
  <li><b>Logs:</b> A designated class is logging every action and error to text files. </li>
  <li><b>JWT Auth:</b> Json Web Token authentication is used to create and verify login tokens for users. </li>
  <li><b>Redis:</b> Redis cache is used for storing some of the data. </li>
  <li><b>SPA Website:</b> The website is a single page application. This was accomplished with React Router. </li>
  <li><b>Responsive Website:</b> The website is mobile friendly. </li>
</ul>

There are 4 types of users, each has different capabilities:

<ul>
  <li><b>Guest:</b> This is an unregistered, anonymous user. Can access all the public of the site: flights search, deals page and flights board. A guest will have to register in order to buy a flight ticket. </li>
  <li><b>Customer:</b> A registered customer can purchase flight tickets and access his private area where he can cancel his tickets or edit his account details. </li>
  <li><b>Airline Company:</b> An airline company can add flights to the database, edit them or cancel them, and also edit their account details. </li>
  <li><b>Administrator:</b> The administrator can manage all the accounts in the website (customers and airlines) and also aprove new airline companies that registered and create an account for them. </li>
</ul>


This is a diagram of the database:

![Database Diagram](https://user-images.githubusercontent.com/45973605/82201713-74b51000-9909-11ea-8d55-f83057b82a12.png)

Screenshots from the website:

![Demo Pictures](https://user-images.githubusercontent.com/45973605/82203292-c5c60380-990b-11ea-8d2a-7325ad30b909.jpg)

The website is hosted on www.ybem.net. Feel free to browse it

I hope you will find this repository valuable for you.

