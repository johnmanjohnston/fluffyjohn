#include <iostream>
#include <chrono>
#include <stdlib.h>
#include <string>
#include <thread>

int iteration = 0;

int main() 
{
    iteration++;
    std::cout << "Running iteration " << iteration << "...\n";
    system("sass storage.scss storage.css");
    std::this_thread::sleep_for(std::chrono::seconds(5));

    main();
}
