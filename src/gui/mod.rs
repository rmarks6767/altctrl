extern crate ncurses;
extern crate gui_lib;

use std::collections::HashMap;
use std::sync::mpsc::{Sender, Receiver};
use serde::Deserialize;
use ncurses::*;

use crate::Event;

#[derive(Clone, Debug, Deserialize)]
pub struct NewWindow {
    id: String,
    content: String,
    x_pos: i32,
    y_pos: i32,
    width: i32,
    height: i32,
}

#[derive(Clone, Debug, Deserialize)]
pub struct WindowID {
    id: String,
}

#[derive(Clone, Debug, Deserialize)]
pub struct LogEvent {
    event: String,
}

#[derive(Clone, Debug)]
pub enum GuiEvent{
    CreateWindow(NewWindow),
    DestroyWindow(WindowID),
    Log(LogEvent),
}

use gui_lib::*;

pub fn launch(tx: Sender<Event>, rx: Receiver<GuiEvent>)
{
    /* Setup ncurses. */
    initscr();
    raw();

    /* Allow for extended keyboard (like F1). */
    keypad(stdscr(), true);
    noecho();

    /* Invisible cursor. */
    curs_set(CURSOR_VISIBILITY::CURSOR_INVISIBLE);

    /* Status/help info. */
    put_pos(0, 0);
    //mvprintw(LINES() - 2, 0, &INSTRUCTIONS);
    refresh();

    // Set up omniscient stuff
    // HashMap of active windows, so that we know what's bonkin'
    let mut windows: std::collections::HashMap<String, WINDOW> = HashMap::new();
    // Log buffer to use for keeping track of command output.
    let mut logbuffer: Vec<String> = Vec::new();
    for _i in 0..5 { logbuffer.push("meme".to_string()); }

    // log("fug".to_string(), &mut logbuffer);
    logbuffer.insert(0, "fug".to_string());
    showlog(&logbuffer);

    /* Get the screen bounds. */
    let mut max_x = 0;
    let mut max_y = 0;
    getmaxyx(stdscr(), &mut max_y, &mut max_x);

    let mut window_height: i32 = 3;
    let mut window_width: i32 = 4;

    /* Start in the center. */
    let mut start_y = (max_y - window_height) / 2;
    let mut start_x = (max_x - window_width) / 2;
    let mut win = create_win("mainwindow".to_string(), start_y, start_x, window_width, window_height, &mut windows);
    
    for message in rx.iter() {

        let mut ch = getch();
        if ch != KEY_F(1) {
            match message {
                GuiEvent::CreateWindow(new_window) => {
                    put_alert(new_window.x_pos, new_window.y_pos, new_window.width, new_window.height, &new_window.id, &new_window.content, &mut windows);
                },
                GuiEvent::DestroyWindow(id) => {
                    close_win(id.id, &mut windows);
                },
                GuiEvent::Log(log_event) => {
                    logbuffer.insert(0, log_event.event);
                }
            }
        } else {
            break;
        }
    }

    endwin();
}

