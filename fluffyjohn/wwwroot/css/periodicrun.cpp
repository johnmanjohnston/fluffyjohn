#include <iostream>
#include <chrono>
#include <stdlib.h>
#include <string>
#include <thread>

// Variable definitions
int iteration = 0;
int iterationCap = 2;

bool capIterations = false;
bool countLogs = true;

// Functions
void Sleep() 
{
    std::this_thread::sleep_for(std::chrono::seconds(5));
}

void RecursiveRun() 
{
    // Verify if we want to run
    if (iteration >= iterationCap && capIterations) 
    {
        std::cout << "Exiting with " << iteration << " log(s)";
        exit(0);
    }

    iteration++;

    if (countLogs) 
    {
        std::cout << "Running iteration " << iteration << "...\n";
    }

    system("sass storage.scss storage.css"); // Main
    Sleep();

    RecursiveRun();
}

int main() 
{
    RecursiveRun();
}
