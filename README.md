<div align="center">
  <a href="https://github.com/TomaSajt/Dusza-PopIt">
    <img src="images/popit128.png" alt="Logo" width="128" height="128">
  </a>
  <h3 align="center">Dusza-PopIt</h3>
  <p align="center">
    An epic PopIt implementation
    <br />
    <a href="https://isze.hu/wp-content/uploads/2017/01/Feladat_regionalis_21-22.pdf"><strong>Explore the docs »</strong></a>
  </p>
</div>


### Built with

* [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
* [dotnet](https://dotnet.microsoft.com/en-us/)


## Usage

You can control the game using a keyboard or by using a mouse.

When in-game use the arrow keys to navigate the cursor around the board. Pressing space will push the selected position. Be careful, after you've pushed one, you can only push the neigbours with the same color. You can also use the mouse to push a cell by left clicking on it. To hand over control to the next player, press the ENTER key.
The game ends when all of the cells have been pushed. In 2 player mode, the winner is the person who didn't push the last cell. With 3 or more player it is the player who would come after the person who pushed the last cell. 

_For more examples, please refer to the [Documentation](https://isze.hu/wp-content/uploads/2017/01/Feladat_regionalis_21-22.pdf)_


## Roadmap

- [x] Add roadmap
- [ ] Better IO/UI
    - [ ] Interface
        - [ ] Menu system
        - [x] Buttons
    - [x] Input
        - [x] Arrow Controls
        - [x] Mouse Controls
            - [x] Detect mouse click
            - [x] Use mouse click
    - [x] Output
        - [x] New Board Visuals
            - [x] 4 color system
            - [x] use color codes instead of ConsoleColor
            - [x] new selection marker
    - [ ] Mixed
        - [ ] Option Prompts
- [x] Reimplement game
    - [x] Use Board class instead of multiple 2D matrices
    - [x] Game Logic
    - [x] Board Generation
        - [x] 0 bends
        - [x] n bends


## License

WTHPL 1.0.0

<p align="right">(<a href="#top">back to top</a>)</p>