#version 450 core

in vec4 color;
in vec3 fragPos;
in vec3 fragNormal;
in vec2 uvCoords;
in flat int matID;

#define SIMPLE 1
#define HARD 2
#define PHONG 3
#define OBJECTS 4

uniform vec3 specularColor;
uniform vec3 lightPosMain, lightPosSecond;
uniform vec3 camPos;

// Per-material properties
#define MAX_MATERIALS 8

uniform int MatProps_renderMode[MAX_MATERIALS];
uniform float MatProps_shininess[MAX_MATERIALS];
uniform float MatProps_diffuseStrength[MAX_MATERIALS];
uniform int TestFrontBack;

layout (location = 0) out vec4 FragColor;

float Diffuse(vec3 normal, vec3 lightDirMain, vec3 lightDirSecond, float secondIntensity, float reverseIntensity) {
    float diffuseMain = max(dot(normal, lightDirMain), 0.0);
    float revDiffMain = max(dot(normal, -lightDirMain), 0.0) * reverseIntensity;
    float diffuseSecond = max(dot(normal, lightDirSecond), 0.0) * secondIntensity;
    float revDiffSecond = max(dot(normal, -lightDirSecond), 0.0) * (secondIntensity + secondIntensity * reverseIntensity) / 2;
    return diffuseMain + diffuseSecond + revDiffMain + revDiffSecond;
}

vec3 FlatNormal(vec3 pos) {
    vec3 dx = dFdx(pos);
    vec3 dy = dFdy(pos);
    return normalize(cross(dx, dy));
}

void main() {
    int renderMode = (TestFrontBack > 0 && gl_FrontFacing == false) ? OBJECTS : MatProps_renderMode[matID];
    if (renderMode == SIMPLE) {
        FragColor = color;

    } else if (renderMode == HARD) {
        float diffuseStrength = MatProps_diffuseStrength[matID];

        vec3 norm = FlatNormal(fragPos);
        vec3 lightDirMain = normalize(lightPosMain - fragPos);
        vec3 lightDirSecond = normalize(lightPosSecond - fragPos);

        float fullDiffuse = Diffuse(norm, lightDirMain, lightDirSecond, 0.6, 0.2);

        vec3 color = color.rgb * ((1 - diffuseStrength) + diffuseStrength * fullDiffuse);
        color = clamp(color, 0.0, 1.0);
        FragColor = vec4(color, 1.0);

    } else if (renderMode == PHONG) {
        float diffuseStrength = MatProps_diffuseStrength[matID];
        float shininess = MatProps_shininess[matID];

        vec3 camDir = normalize(camPos - fragPos);
        vec3 lightDirMain = normalize(-lightPosMain);
        vec3 lightDirSecond = normalize(-lightPosSecond);
        vec3 N = normalize(fragNormal);
        
        float fullDiffuse = Diffuse(N, lightDirMain, lightDirSecond, 0.3, 0.05);

        //float diffuseHalf = 0.5 * diffuse + 0.5;
        //diffuseHalf = diffuseHalf * diffuseHalf;

        //vec3 VertexToEye = camDir;
        //vec3 ReflectLight = reflect(-lightDirMain, normalize(fragNormal));
        //float specFactor = dot(VertexToEye, ReflectLight);
        //specFactor = max(specFactor, 0.0);
        //specFactor = pow(specFactor, shininess);

        vec3 H = normalize(lightDirMain + camDir);
        float specFactor = dot(N, H);
        specFactor = pow(max(specFactor, 0.0), shininess);

        float fresnel = pow(1.0 - max(dot(camDir, camDir), 0.0), 3.0);

        //float rim = smoothstep(0.0, 1.0, 1.0 - max(dot(camDir, camDir), 0.0));

        vec3 color = color.rgb * ((1 - diffuseStrength) + diffuseStrength * fullDiffuse)
            + specularColor * specFactor;
            //+ rim;

        FragColor = vec4(color, 1.0);

    } else if (renderMode == OBJECTS) {
        // Debugging mode to visualize different meshes by color
        vec3 color = vec3(0.8, 0.8, 0.8);

        switch (matID) {
            case 0: color = vec3(1.0, 0.0, 0.0); break; // Red
            case 1: color = vec3(0.0, 1.0, 0.0); break; // Green
            case 2: color = vec3(0.0, 0.0, 1.0); break; // Blue
            case 3: color = vec3(1.0, 1.0, 0.0); break; // Yellow
            case 4: color = vec3(1.0, 0.0, 1.0); break; // Magenta
            case 5: color = vec3(0.0, 1.0, 1.0); break; // Cyan
            default:
                float R = (1 + matID % 4) / 4;
                float G = (1 + (matID / 4) % 4) / 4;
                float B = (1 + (matID / 16) % 4) / 4;
                color = vec3(R, G, B); // Unique color
                break;
        }

        if (gl_FrontFacing == false) {
            // Create stripes on backfaces for better visualization based on pixel position (gl_FragCoord)
            float stripeWidth = 4.0;
            float stripe = mod(floor(gl_FragCoord.x / stripeWidth) + floor(gl_FragCoord.y / stripeWidth), 2.0);
            if (stripe < 1.0)
                color *= 0.4; // Darken color for stripes
        }

        vec3 norm = FlatNormal(fragPos);
        vec3 lightDirMain = normalize(lightPosMain - fragPos);
        vec3 lightDirSecond = normalize(lightPosSecond - fragPos);
        float fullDiffuse = Diffuse(norm, lightDirMain, lightDirSecond, 0.6, 0.2);

        FragColor = vec4(color * fullDiffuse, 1.0);
    } else {
        // UNKNOWN MODE: Visualize crazy colors based on position to spot errors
        float R = abs(sin(fragPos.x * 10.0) / 2 + 0.5);
        float G = abs(sin(fragPos.y * 10.0) / 2 + 0.5);
        float B = abs(sin(fragPos.z * 10.0) / 2 + 0.5);
        FragColor = vec4(R, G, B, 1.0);
    }
}