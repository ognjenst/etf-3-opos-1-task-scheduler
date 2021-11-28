
Task Scheduler
---
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
--

This was the first assignment for the "Selected Chapters from Operating Systems" subject: Task Scheduler

Basic idea was to create a Task Scheduler imitation, and show how does a scheduler work. I tried to make it work with real tasks in .Net Core.

## Method
The main idea was to use a Collection of Tasks, give each one work to do, and then execute them sequentially, and in parallelisation, if there is more than one thread available. 

There is an option to add tasks with higher priority, which would jump the queue. 

## Requirements
You will need [Visual Studio](https://visualstudio.microsoft.com/) to run this project.

## Configuration
You can configure how many threads are available at the app launch, and how many tasks there will be in the queue. 

## Improvements
Since this was a proof of concept, I did not finish all the requirements.
There are a few possible improvements:
* Add preemptive scheduling
* Save current thread state, pause, and continue later
