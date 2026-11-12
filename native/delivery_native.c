#if defined(_WIN32)
#define API __declspec(dllexport)
#else
#define API __attribute__((visibility("default")))
#endif
API int delivery_zone(int distance_meters) {
    if (distance_meters < 0 || distance_meters > 200000) return 0;
    if (distance_meters <= 5000) return 1;  
    if (distance_meters <= 12000) return 2; 
    return 3;                               
}
