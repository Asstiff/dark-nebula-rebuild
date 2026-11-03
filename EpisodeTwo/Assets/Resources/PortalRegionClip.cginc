// Only the new tall-screen bands are restricted. The original 320x480 view
// keeps its authored rendering, including the native portal animation.
float4 _PortalRegion;
float PortalRegionAlpha(float worldZ, float4 screen)
{
    float screenY = screen.y / screen.w;
    if (abs(screenY - 0.5) <= 0.5 * _PortalRegion.w) return 1.0;
    clip(worldZ - _PortalRegion.x);
    clip(_PortalRegion.y - worldZ);
    return _PortalRegion.z;
}
