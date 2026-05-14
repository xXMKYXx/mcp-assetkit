-- mcp-assetkit MariaDB schema + seed data
-- MSP-style asset inventory: clients own assets (laptops, servers, network gear, etc.)

CREATE TABLE clients (
  id           VARCHAR(36)  PRIMARY KEY,
  name         VARCHAR(200) NOT NULL,
  industry     VARCHAR(100) NOT NULL,
  created_at   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE assets (
  id                   VARCHAR(36)  PRIMARY KEY,
  client_id            VARCHAR(36)  NOT NULL,
  asset_type           VARCHAR(50)  NOT NULL,
  vendor               VARCHAR(100) NOT NULL,
  model                VARCHAR(100) NOT NULL,
  serial               VARCHAR(100) NOT NULL,
  status               VARCHAR(30)  NOT NULL,
  warranty_expires_at  DATE         NULL,
  created_at           DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_assets_client FOREIGN KEY (client_id) REFERENCES clients(id)
);

CREATE INDEX idx_assets_client    ON assets(client_id);
CREATE INDEX idx_assets_warranty  ON assets(warranty_expires_at);
CREATE INDEX idx_assets_status    ON assets(status);
CREATE INDEX idx_assets_type      ON assets(asset_type);

-- Clients
INSERT INTO clients (id, name, industry) VALUES
 ('c1','Acme Healthcare',          'Healthcare'),
 ('c2','BlueRiver Logistics',      'Logistics'),
 ('c3','Northwind Manufacturing',  'Manufacturing'),
 ('c4','PinePoint Legal',          'Legal'),
 ('c5','Summit Retail',            'Retail');

-- Assets — varied vendor / type / status / warranty distribution
INSERT INTO assets (id, client_id, asset_type, vendor, model, serial, status, warranty_expires_at) VALUES
 -- Acme Healthcare
 ('a-0001','c1','Laptop',       'Dell',     'Latitude 5440',   'DLT-5440-001','Active',         '2027-03-15'),
 ('a-0002','c1','Laptop',       'Dell',     'Latitude 5440',   'DLT-5440-002','Active',         '2027-03-15'),
 ('a-0003','c1','Laptop',       'Lenovo',   'ThinkPad T14',    'LTP-T14-101', 'Active',         '2026-11-20'),
 ('a-0004','c1','Desktop',      'HP',       'EliteDesk 800',   'HPE-ED800-44','Active',         '2026-08-01'),
 ('a-0005','c1','Server',       'HPE',      'ProLiant DL380',  'HPE-DL380-99','Active',         '2026-08-01'),
 ('a-0006','c1','Server',       'Dell',     'PowerEdge R750',  'DPE-R750-22', 'Active',         '2027-05-10'),
 ('a-0007','c1','Firewall',     'Fortinet', 'FortiGate 100F',  'FG100F-AC1',  'Active',         '2025-12-31'),
 ('a-0008','c1','NetworkSwitch','Cisco',    'Catalyst 9300',   'C9300-AC-01', 'Active',         '2027-01-15'),
 ('a-0009','c1','Printer',      'Brother',  'HL-L8360CDW',     'BRT-L836-77', 'InRepair',       '2024-06-30'),
 ('a-0010','c1','Laptop',       'Apple',    'MacBook Pro 14',  'MBP14-2023-1','Active',         '2026-09-15'),
 ('a-0011','c1','Laptop',       'Apple',    'MacBook Pro 14',  'MBP14-2023-2','Decommissioned', '2024-09-15'),
 ('a-0012','c1','Desktop',      'Dell',     'OptiPlex 7090',   'DOP-7090-12', 'Active',         '2025-04-20'),

 -- BlueRiver Logistics
 ('a-0013','c2','Laptop',       'Lenovo',   'ThinkPad X1',     'LTP-X1-201',  'Active',         '2027-02-28'),
 ('a-0014','c2','Laptop',       'Lenovo',   'ThinkPad X1',     'LTP-X1-202',  'Active',         '2027-02-28'),
 ('a-0015','c2','Laptop',       'Dell',     'XPS 13',          'DXP-13-301',  'Active',         '2026-12-01'),
 ('a-0016','c2','Server',       'Dell',     'PowerEdge R640',  'DPE-R640-77', 'Active',         '2026-05-15'),
 ('a-0017','c2','NetworkSwitch','Juniper',  'EX4400',          'JNX-EX44-01', 'Active',         '2027-08-22'),
 ('a-0018','c2','NetworkSwitch','Juniper',  'EX4400',          'JNX-EX44-02', 'Active',         '2027-08-22'),
 ('a-0019','c2','Firewall',     'Fortinet', 'FortiGate 60F',   'FG60F-BR1',   'Active',         '2026-04-10'),
 ('a-0020','c2','Printer',      'Canon',    'imageRUNNER 1643','CNN-IR1643-1','Active',         '2025-10-05'),
 ('a-0021','c2','Desktop',      'HP',       'ProDesk 600',     'HPP-PD600-1', 'Decommissioned', '2023-11-20'),
 ('a-0022','c2','Laptop',       'Dell',     'Latitude 7440',   'DLT-7440-301','Active',         '2026-07-30'),

 -- Northwind Manufacturing
 ('a-0023','c3','Server',       'HPE',      'ProLiant DL360',  'HPE-DL360-11','Active',         '2026-11-30'),
 ('a-0024','c3','Server',       'HPE',      'ProLiant DL360',  'HPE-DL360-12','Active',         '2026-11-30'),
 ('a-0025','c3','Server',       'HPE',      'ProLiant DL580',  'HPE-DL580-01','Active',         '2027-04-15'),
 ('a-0026','c3','NetworkSwitch','Cisco',    'Catalyst 9500',   'C9500-NW-01', 'Active',         '2027-06-01'),
 ('a-0027','c3','NetworkSwitch','Cisco',    'Catalyst 9300',   'C9300-NW-02', 'Active',         '2027-06-01'),
 ('a-0028','c3','Firewall',     'Palo Alto','PA-3220',         'PA-3220-NW1', 'Active',         '2026-09-20'),
 ('a-0029','c3','Firewall',     'Palo Alto','PA-3220',         'PA-3220-NW2', 'Active',         '2026-09-20'),
 ('a-0030','c3','Desktop',      'Lenovo',   'ThinkCentre M70', 'LTC-M70-401', 'Active',         '2026-03-01'),
 ('a-0031','c3','Desktop',      'Lenovo',   'ThinkCentre M70', 'LTC-M70-402', 'Active',         '2026-03-01'),
 ('a-0032','c3','Laptop',       'Dell',     'Precision 5680',  'DPR-5680-501','Active',         '2027-01-10'),
 ('a-0033','c3','Printer',      'Brother',  'MFC-L9570CDW',    'BRT-9570-22', 'InRepair',       '2024-02-15'),
 ('a-0034','c3','Laptop',       'HP',       'EliteBook 840',   'HPE-EB840-1', 'Decommissioned', '2023-06-01'),

 -- PinePoint Legal
 ('a-0035','c4','Laptop',       'Apple',    'MacBook Pro 16',  'MBP16-PL-01', 'Active',         '2026-12-15'),
 ('a-0036','c4','Laptop',       'Apple',    'MacBook Pro 16',  'MBP16-PL-02', 'Active',         '2026-12-15'),
 ('a-0037','c4','Laptop',       'Apple',    'MacBook Air 13',  'MBA13-PL-01', 'Active',         '2027-03-20'),
 ('a-0038','c4','Desktop',      'Apple',    'iMac 24',         'IMC24-PL-01', 'Active',         '2026-10-10'),
 ('a-0039','c4','Server',       'Dell',     'PowerEdge T440',  'DPE-T440-PL', 'Active',         '2026-06-15'),
 ('a-0040','c4','NetworkSwitch','Aruba',    'CX 6300',         'ARB-CX63-PL', 'Active',         '2027-05-01'),
 ('a-0041','c4','Firewall',     'Fortinet', 'FortiGate 80F',   'FG80F-PL-01', 'Active',         '2026-08-12'),
 ('a-0042','c4','Printer',      'Canon',    'imageCLASS MF',   'CNN-MFPL-01', 'Active',         '2025-11-30'),

 -- Summit Retail
 ('a-0043','c5','Laptop',       'HP',       'ProBook 450',     'HPB-450-SR1', 'Active',         '2026-04-25'),
 ('a-0044','c5','Laptop',       'HP',       'ProBook 450',     'HPB-450-SR2', 'Active',         '2026-04-25'),
 ('a-0045','c5','Laptop',       'HP',       'ProBook 450',     'HPB-450-SR3', 'Active',         '2026-04-25'),
 ('a-0046','c5','Desktop',      'Dell',     'OptiPlex 3000',   'DOP-3000-SR', 'Active',         '2026-01-15'),
 ('a-0047','c5','Server',       'Dell',     'PowerEdge R350',  'DPE-R350-SR', 'Active',         '2025-09-01'),
 ('a-0048','c5','NetworkSwitch','Cisco',    'Catalyst 1000',   'C1000-SR-01', 'Active',         '2026-02-28'),
 ('a-0049','c5','NetworkSwitch','Cisco',    'Catalyst 1000',   'C1000-SR-02', 'Active',         '2026-02-28'),
 ('a-0050','c5','Firewall',     'SonicWall','TZ370',           'SW-TZ370-SR', 'Active',         '2026-11-08'),
 ('a-0051','c5','Printer',      'Brother',  'HL-L2370DW',      'BRT-2370-SR', 'Active',         '2024-12-01'),
 ('a-0052','c5','Laptop',       'Lenovo',   'IdeaPad 5',       'LIP-5-SR-01', 'Decommissioned', '2023-08-20');
