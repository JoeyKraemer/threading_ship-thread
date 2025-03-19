CREATE TABLE Driver (
                        driver_id SERIAL PRIMARY KEY,
                        name VARCHAR(255) NOT NULL,
                        contact_number VARCHAR(50),
                        email VARCHAR(255) UNIQUE,
                        license_type VARCHAR(2) CHECK (license_type IN ('B', 'BE', 'C'))
);

CREATE TABLE Cargo (
                       cargo_id SERIAL PRIMARY KEY,
                       description VARCHAR(255) NOT NULL,
                       weight DOUBLE PRECISION,
                       destination JSONB NOT NULL,
                       status VARCHAR(20) CHECK (status IN ('In Transit', 'Ready to Ship', 'Delivered', 'Lost')),
                       CONSTRAINT valid_destination CHECK (
                           destination ? 'latitude' AND destination ? 'longitude'
)
    );

CREATE TABLE Truck (
                       truck_id SERIAL PRIMARY KEY,
                       license_plate VARCHAR(50) NOT NULL,
                       driver_id INT,
                       current_location JSONB NOT NULL,
                       status VARCHAR(20) CHECK (status IN ('Available', 'In Transit', 'Damaged', 'Maintenance')),
                       cargo_id INT,
                       FOREIGN KEY (driver_id) REFERENCES Driver(driver_id) ON DELETE NO ACTION,
                       FOREIGN KEY (cargo_id) REFERENCES Cargo(cargo_id) ON DELETE NO ACTION,
                       CONSTRAINT valid_location CHECK (
                           current_location ? 'latitude' AND current_location ? 'longitude'
)
    );

CREATE TABLE Delivery (
                          delivery_id SERIAL PRIMARY KEY,
                          cargo_id INT,
                          truck_id INT,
                          start_time TIMESTAMP,
                          end_time TIMESTAMP,
                          delivery_status VARCHAR(20) CHECK (delivery_status IN ('On Route', 'Delivered')),
                          FOREIGN KEY (cargo_id) REFERENCES Cargo(cargo_id) ON DELETE NO ACTION,
                          FOREIGN KEY (truck_id) REFERENCES Truck(truck_id) ON DELETE NO ACTION
);

CREATE TABLE LocationUpdate (
                                update_id SERIAL PRIMARY KEY,
                                truck_id INT,
                                timestamp TIMESTAMP NOT NULL,
                                latitude DOUBLE PRECISION,
                                longitude DOUBLE PRECISION,
                                FOREIGN KEY (truck_id) REFERENCES Truck(truck_id) ON DELETE NO ACTION
);

CREATE TABLE Event (
                       event_id SERIAL PRIMARY KEY,
                       truck_id INT,
                       event_type VARCHAR(20) CHECK (event_type IN ('Accident', 'Delay', 'Maintenance')),
                       description VARCHAR(255),
                       timestamp TIMESTAMP NOT NULL,
                       FOREIGN KEY (truck_id) REFERENCES Truck(truck_id) ON DELETE NO ACTION
);
