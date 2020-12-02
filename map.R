library(tidyverse)
library(ggrepel)

city_names <- c(
    "Racine",
    "Milwaukee",
    "Sheboygan",
    "Fond du Lac",
    "Green Bay",
    "La Crosse",
    "Mauston",
    "Madison"
)

nz <- map_data("state", region = "wisconsin")
schools <- read.csv("schools.csv") %>%
    rename(name = school) %>%
    mutate(type = factor("school"))
cities <- read.csv("wisconsin-cities.csv") %>%
    rename(name = city) %>%
    filter(is.element(name, city_names)) %>%
    mutate(type = factor("city"))
locations <- rbind(schools, cities)

ggplot(nz, aes(long, lat)) +
    coord_quickmap() +
    geom_polygon(fill = "#c3edaf", color = "black") +
    geom_point(data = locations, aes(lng, lat, shape = type, color = type), size = 2) +
    geom_text_repel(data = filter(locations, type == "city"), aes(lng, lat, label = name), seed = 1) +
    ggtitle("Kriete Group Locations and Technical Colleges of Wisconsin") +
    theme_void() +
    theme(legend.title = element_blank())
ggsave("map-R.png")
