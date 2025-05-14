# Campus Love

## DDL

```sql
-- Crear base de datos
DROP DATABASE IF EXISTS campus_love;
CREATE DATABASE campus_love
WITH ENCODING 'UTF8'
TEMPLATE=template0;

\c campus_love;

-- 1. Geography

CREATE TABLE country (
  id SERIAL,
  name VARCHAR(100) NOT NULL,
  CONSTRAINT pk_country PRIMARY KEY (id)
);

CREATE TABLE region (
  id SERIAL,
  name VARCHAR(100) NOT NULL,
  country_id INTEGER NOT NULL,
  CONSTRAINT pk_region PRIMARY KEY (id),
  CONSTRAINT fk_region_country FOREIGN KEY (country_id)
    REFERENCES country(id)
);

CREATE TABLE city (
  id SERIAL,
  name VARCHAR(100) NOT NULL,
  region_id INTEGER NOT NULL,
  CONSTRAINT pk_city PRIMARY KEY (id),
  CONSTRAINT fk_city_region FOREIGN KEY (region_id)
    REFERENCES region(id)
);

CREATE TABLE address (
  id SERIAL,
  street VARCHAR(255) NOT NULL,
  building_number VARCHAR(20) NOT NULL,
  postal_code VARCHAR(20),
  city_id INTEGER NOT NULL,
  additional_info TEXT,
  CONSTRAINT pk_address PRIMARY KEY (id),
  CONSTRAINT fk_address_city FOREIGN KEY (city_id)
    REFERENCES city(id)
);

-- 2. Domain Catalogs

CREATE TABLE gender (
  gender_id SERIAL,
  description VARCHAR(50) UNIQUE NOT NULL,
  CONSTRAINT pk_gender PRIMARY KEY (gender_id)
);

CREATE TABLE sexual_orientation (
  orientation_id SERIAL,
  description VARCHAR(50) UNIQUE NOT NULL,
  CONSTRAINT pk_sexual_orientation PRIMARY KEY (orientation_id)
);

CREATE TABLE career (
  career_id SERIAL,
  name VARCHAR(100) UNIQUE NOT NULL,
  CONSTRAINT pk_career PRIMARY KEY (career_id)
);

CREATE TABLE interest (
  interest_id SERIAL,
  description VARCHAR(100) UNIQUE NOT NULL,
  CONSTRAINT pk_interest PRIMARY KEY (interest_id)
);

CREATE TABLE interaction_type (
  id SERIAL,
  description VARCHAR(30) UNIQUE NOT NULL,
  CONSTRAINT pk_interaction_type PRIMARY KEY (id)
);

CREATE TABLE user_type (
  id SERIAL,
  description VARCHAR(30)UNIQUE NOT NULL,
  CONSTRAINT pk_user_type PRIMARY KEY (id)
)

-- 3. Users and Authentication

CREATE TABLE app_user (
  user_id SERIAL,
  name VARCHAR(100) NOT NULL,
  age INT CHECK (age >= 18),
  email VARCHAR(100) UNIQUE NOT NULL,
  password_hash TEXT NOT NULL,
  gender_id INT NOT NULL,
  user_type_id INT NOT NULL,
  CONSTRAINT pk_app_user PRIMARY KEY (user_id),
  CONSTRAINT fk_app_user_gender FOREIGN KEY (gender_id)
    REFERENCES gender(gender_id),
  CONSTRAINT fk_app_user_type FOREIGN KEY (user_type_id) REFERENCES user_type(id)
);

-- 4. Preferences (including sexual orientation)

CREATE TABLE preference (
  preference_id SERIAL,
  orientation_id INT,
  min_age INT NOT NULL CHECK (min_age >= 18),
  max_age INT NOT NULL,
  CONSTRAINT pk_preference PRIMARY KEY (preference_id),
  CONSTRAINT fk_preference_orientation FOREIGN KEY (orientation_id)
    REFERENCES sexual_orientation(orientation_id)
);

-- 5. Public User Profile

CREATE TABLE user_profile (
  user_id INT,
  preference_id INT,
  profile_text TEXT,
  address_id INT NOT NULL,
  verified BOOLEAN DEFAULT FALSE,
  status VARCHAR(20) DEFAULT 'active',
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT pk_user_profile PRIMARY KEY (user_id),
  CONSTRAINT fk_user_profile_user FOREIGN KEY (user_id)
    REFERENCES app_user(user_id),
  CONSTRAINT fk_user_profile_preference FOREIGN KEY (preference_id)
    REFERENCES preference(preference_id),
  CONSTRAINT fk_user_profile_address FOREIGN KEY (address_id)
    REFERENCES address(id)
);

-- 6. Many-to‑Many Relationships

CREATE TABLE user_career (
  user_id INT,
  career_id INT,
  CONSTRAINT pk_user_career PRIMARY KEY (user_id, career_id),
  CONSTRAINT fk_user_career_user FOREIGN KEY (user_id)
    REFERENCES app_user(user_id),
  CONSTRAINT fk_user_career_career FOREIGN KEY (career_id)
    REFERENCES career(career_id)
);

CREATE TABLE user_interest (
  user_id INT,
  interest_id INT,
  CONSTRAINT pk_user_interest PRIMARY KEY (user_id, interest_id),
  CONSTRAINT fk_user_interest_user FOREIGN KEY (user_id)
    REFERENCES app_user(user_id),
  CONSTRAINT fk_user_interest_interest FOREIGN KEY (interest_id)
    REFERENCES interest(interest_id)
);

-- 7. Interactions (Like / Dislike)

CREATE TABLE interaction (
  interaction_id SERIAL,
  source_user_id INT,
  target_user_id INT,
  interaction_type_id INT,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT pk_interaction PRIMARY KEY (interaction_id),
  CONSTRAINT fk_interaction_source FOREIGN KEY (source_user_id)
    REFERENCES app_user(user_id),
  CONSTRAINT fk_interaction_target FOREIGN KEY (target_user_id)
    REFERENCES app_user(user_id),
  CONSTRAINT fk_interaction_type FOREIGN KEY (interaction_type_id)
    REFERENCES interaction_type(id)
);

-- 8. Matches and M:N Relationship

CREATE TABLE match (
  match_id SERIAL,
  user1_id INT,
  user2_id INT,
  matched_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT pk_match PRIMARY KEY (match_id),
  CONSTRAINT fk_match_user1 FOREIGN KEY (user1_id)
    REFERENCES app_user(user_id),
  CONSTRAINT fk_match_user2 FOREIGN KEY (user2_id)
    REFERENCES app_user(user_id)
);

CREATE TABLE user_match (
  user_id INT,
  match_id INT,
  CONSTRAINT pk_user_match PRIMARY KEY (user_id, match_id),
  CONSTRAINT fk_user_match_user FOREIGN KEY (user_id)
    REFERENCES app_user(user_id),
  CONSTRAINT fk_user_match_match FOREIGN KEY (match_id)
    REFERENCES match(match_id)
);

-- 9. Daily Interaction Credits

CREATE TABLE interaction_credits (
  credit_id SERIAL,
  user_id INT,
  on_date DATE NOT NULL,
  likes_available INT NOT NULL DEFAULT 5,
  CONSTRAINT pk_interaction_credits PRIMARY KEY (credit_id),
  CONSTRAINT fk_interaction_credits_user FOREIGN KEY (user_id)
    REFERENCES app_user(user_id),
  CONSTRAINT uq_interaction_credits_user_date UNIQUE (user_id, on_date)
);

-- 10. User Statistics

CREATE TABLE user_statistics (
  user_id INT,
  likes_given INT DEFAULT 0,
  likes_received INT DEFAULT 0,
  total_matches INT DEFAULT 0,
  last_interaction_at TIMESTAMP,
  CONSTRAINT pk_user_statistics PRIMARY KEY (user_id),
  CONSTRAINT fk_user_statistics_user FOREIGN KEY (user_id)
    REFERENCES app_user(user_id)
);

```

 

## DML

```sql
\c campus_love;

-- ========================================================
-- DML: Populate country, region and city tables
-- Countries: Colombia, Mexico, Argentina, United States
-- ========================================================

-- 1. Insert countries
INSERT INTO country (name) VALUES
  ('Colombia'),
  ('Mexico'),
  ('Argentina'),
  ('United States');

-- 2. Insert regions (departments / states), referencing country_id
INSERT INTO region (name, country_id) VALUES
  -- Colombia (country_id = 1)
  ('Cundinamarca',       1),
  ('Antioquia',          1),
  ('Valle del Cauca',    1),
  ('Atlántico',          1),
  ('Santander',          1),
  ('Bolívar',            1),
  ('Nariño',             1),
  ('Magdalena',          1),
  ('Cesar',              1),
  ('Meta',               1),

  -- Mexico (country_id = 2)
  ('Ciudad de México',   2),
  ('Jalisco',            2),
  ('Nuevo León',         2),
  ('Veracruz',           2),
  ('Puebla',             2),

  -- Argentina (country_id = 3)
  ('Buenos Aires',       3),
  ('Córdoba',            3),
  ('Santa Fe',           3),
  ('Mendoza',            3),
  ('Tucumán',            3),

  -- United States (country_id = 4)
  ('California',         4),
  ('New York',           4),
  ('Texas',              4),
  ('Florida',            4),
  ('Illinois',           4);

-- 3. Insert cities, referencing region_id in insertion order
INSERT INTO city (name, region_id) VALUES
  -- Colombia (region_id = 1–10)
  ('Bogotá',               1),
  ('Soacha',               1),
  ('Chía',                 1),

  ('Medellín',             2),
  ('Envigado',             2),
  ('Bello',                2),

  ('Cali',                 3),
  ('Palmira',              3),
  ('Buenaventura',         3),

  ('Barranquilla',         4),
  ('Soledad',              4),
  ('Puerto Colombia',      4),

  ('Bucaramanga',          5),
  ('Floridablanca',        5),
  ('Piedecuesta',          5),
  ('Girón',                5),

  ('Cartagena',            6),
  ('Magangué',             6),
  ('Turbaco',              6),

  ('Pasto',                7),
  ('Ipiales',              7),
  ('Tumaco',               7),

  ('Santa Marta',          8),
  ('Ciénaga',              8),
  ('Fundación',            8),

  ('Valledupar',           9),
  ('Aguachica',            9),
  ('La Paz',               9),

  ('Villavicencio',       10),
  ('Acacías',             10),
  ('Granada',             10),

  -- Mexico (region_id = 11–15)
  ('Ciudad de México',    11),
  ('Coyoacán',            11),
  ('Xochimilco',          11),

  ('Guadalajara',         12),
  ('Zapopan',             12),
  ('Tlaquepaque',         12),

  ('Monterrey',           13),
  ('Guadalupe',           13),
  ('San Nicolás de los Garza', 13),

  ('Veracruz',            14),
  ('Xalapa',              14),
  ('Coatzacoalcos',       14),

  ('Puebla',              15),
  ('Tehuacán',            15),
  ('Atlixco',             15),

  -- Argentina (region_id = 16–20)
  ('La Plata',            16),
  ('Mar del Plata',       16),
  ('Bahía Blanca',        16),

  ('Córdoba',             17),
  ('Río Cuarto',          17),
  ('Villa Carlos Paz',    17),

  ('Santa Fe',            18),
  ('Rosario',             18),
  ('Rafaela',             18),

  ('Mendoza',             19),
  ('San Rafael',          19),
  ('Godoy Cruz',          19),

  ('San Miguel de Tucumán', 20),
  ('Tafí Viejo',          20),
  ('Concepción',          20),

  -- United States (region_id = 21–25)
  ('Los Angeles',         21),
  ('San Francisco',       21),
  ('San Diego',           21),

  ('New York City',       22),
  ('Buffalo',             22),
  ('Rochester',           22),

  ('Houston',             23),
  ('Dallas',              23),
  ('Austin',              23),

  ('Miami',               24),
  ('Orlando',             24),
  ('Tampa',               24),

  ('Chicago',             25),
  ('Springfield',         25),
  ('Naperville',          25);

-- ========================================================
-- DML: Populate domain tables
-- Tables: gender, sexual_orientation, career, interest, interaction_type
-- ========================================================

-- 1. Gender (Female and Male)
INSERT INTO gender (description) VALUES
  ('Female'),
  ('Male');

-- 2. Sexual orientation (Heterosexual, Homosexual, Bisexual)
INSERT INTO sexual_orientation (description) VALUES
  ('Heterosexual'),
  ('Homosexual'),
  ('Bisexual');

-- 3. Career (some common fields of study)
INSERT INTO career (name) VALUES
  ('Systems Engineering'),
  ('Medicine'),
  ('Law'),
  ('Business Administration'),
  ('Psychology'),
  ('Industrial Engineering'),
  ('Social Communication'),
  ('Architecture'),
  ('Public Accounting'),
  ('Economics');

-- 4. Interest (common interests)
INSERT INTO interest (description) VALUES
  ('Sports'),
  ('Music'),
  ('Reading'),
  ('Travel'),
  ('Cinema'),
  ('Technology'),
  ('Gastronomy'),
  ('Photography'),
  ('Video Games'),
  ('Art'),
  ('Cryptocurrencies');

-- 5. Interaction type (Like / Dislike)
INSERT INTO interaction_type (description) VALUES
  ('Like'),
  ('Dislike');

```

