-- phpMyAdmin SQL Dump
-- version 4.8.5
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 25-04-2025 a las 23:01:08
-- Versión del servidor: 10.1.38-MariaDB
-- Versión de PHP: 5.6.40

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria_deramo`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `Id` int(11) NOT NULL,
  `IdInquilino` int(11) NOT NULL,
  `IdInmueble` int(11) NOT NULL,
  `MontoMensual` double NOT NULL,
  `FechaDesde` datetime NOT NULL,
  `FechaHasta` datetime NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  `FechaTerminacionAnticipada` datetime NOT NULL,
  `MontoMulta` decimal(10,0) NOT NULL,
  `MultaPagada` tinyint(4) NOT NULL,
  `UsuarioAltaId` int(11) DEFAULT NULL,
  `UsuarioBajaId` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish2_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`Id`, `IdInquilino`, `IdInmueble`, `MontoMensual`, `FechaDesde`, `FechaHasta`, `Activo`, `FechaTerminacionAnticipada`, `MontoMulta`, `MultaPagada`, `UsuarioAltaId`, `UsuarioBajaId`) VALUES
(2, 4, 22, 500000, '2025-04-03 04:03:00', '2025-04-11 20:02:00', 0, '0000-00-00 00:00:00', '0', 0, 3, NULL),
(5, 5, 13, 800000, '2025-04-05 21:11:00', '2025-05-10 21:11:00', 1, '0000-00-00 00:00:00', '0', 0, NULL, NULL),
(6, 6, 21, 100, '2025-04-05 23:30:00', '2025-05-31 21:27:00', 1, '0000-00-00 00:00:00', '0', 0, NULL, NULL),
(7, 7, 29, 10, '2025-04-10 20:36:00', '2025-05-10 20:36:00', 1, '0000-00-00 00:00:00', '0', 0, NULL, NULL),
(8, 8, 21, 6, '2025-04-10 20:46:00', '2025-05-10 20:46:00', 1, '0000-00-00 00:00:00', '0', 0, NULL, NULL),
(12, 5, 29, 5, '2025-04-11 21:34:00', '2025-05-10 21:34:00', 0, '2025-04-18 00:00:00', '5', 1, NULL, 3),
(13, 7, 29, 6, '2025-04-11 16:26:00', '2025-05-11 16:26:00', 1, '0000-00-00 00:00:00', '0', 0, NULL, NULL),
(14, 6, 14, 6, '2025-04-11 16:27:00', '2025-05-10 16:27:00', 1, '0000-00-00 00:00:00', '0', 0, NULL, NULL),
(16, 8, 11, 1, '2025-04-18 00:19:00', '2025-04-25 00:19:00', 0, '2025-04-18 00:00:00', '2', 1, 3, 3),
(17, 5, 2, 2, '2025-04-18 01:28:00', '2025-04-25 01:28:00', 1, '0000-00-00 00:00:00', '0', 0, 3, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `imagenes`
--

CREATE TABLE `imagenes` (
  `Id` int(11) NOT NULL,
  `InmuebleId` int(11) NOT NULL,
  `Url` varchar(500) COLLATE utf8mb4_spanish2_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish2_ci;

--
-- Volcado de datos para la tabla `imagenes`
--

INSERT INTO `imagenes` (`Id`, `InmuebleId`, `Url`) VALUES
(43, 2, '/Uploads/Inmuebles/2/3fdde5bc-5191-44b8-b541-d310f5e5e75e.jpg'),
(51, 2, '/Uploads/Inmuebles/2/374ace87-15b3-4804-b64e-5060d5747c59.jpg'),
(52, 2, '/Uploads/Inmuebles/2/59fb8fc8-9913-4f05-b466-5b3ca78c9d04.jpg'),
(53, 16, '/Uploads/Inmuebles/16/a837caf8-2a49-4965-ba9c-59cb23afd00c.jpg');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `Id` int(11) NOT NULL,
  `Direccion` varchar(255) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Uso` varchar(100) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Tipo` varchar(100) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Precio` double NOT NULL,
  `Ambientes` int(10) NOT NULL,
  `Superficie` int(100) NOT NULL,
  `Latitud` decimal(65,0) NOT NULL,
  `Longitud` decimal(65,0) NOT NULL,
  `PropietarioId` int(11) NOT NULL,
  `Portada` varchar(255) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Activo` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish2_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`Id`, `Direccion`, `Uso`, `Tipo`, `Precio`, `Ambientes`, `Superficie`, `Latitud`, `Longitud`, `PropietarioId`, `Portada`, `Activo`) VALUES
(2, 'Sapee', 'Domestico', 'Casa', 80000000, 4, 500, '99', '78', 1, '/Uploads/Inmuebles\\portada_2.jpg', 1),
(11, 'Allá', 'Domestico', 'Casa', 80000000, 4, 556, '67', '67', 5, '/Uploads/Inmuebles\\portada_11.jpg', 1),
(13, 'Nose o si', 'Domestico', 'Casa', 1233, 4, 567, '0', '0', 1, '/Uploads/Inmuebles\\portada_13.jpg', 1),
(14, 'Sur', 'Domestico', 'Departamento', 963258, 2, 150, '10', '9', 2, '/Uploads/Inmuebles\\portada_14.jpg', 1),
(16, 'Este', 'Cancha', 'Terreno', 852147963, 1, 800, '9862', '7854', 4, '/Uploads/Inmuebles\\portada_16.jpg', 1),
(19, 'Oeste', 'Oficina', 'Campo', 23, 4, 567, '89', '9636', 4, '/Uploads/Inmuebles\\portada_19.jpg', 1),
(21, 'SurOeste', 'comercial', 'Local comercial', 10000000, 3, 500, '96654', '7845', 1, '/Uploads/Inmuebles\\portada_21.jpg', 0),
(22, 'Lejos', 'comercial', 'Local comercial', 80000000, 5, 500, '13', '90', 2, '/Uploads/Inmuebles\\portada_22.jfif', 1),
(29, 'Barrio Norte', 'Domestico', 'Casa', 1, 4, 800, '0', '0', 6, '/Uploads/Inmuebles\\portada_29.png', 0),
(30, 'Del Oeste', 'Doméstico', 'Casa', 400000, 3, 400, '0', '0', 1, '', 0),
(36, 'Al otro lado', 'Local comercial', 'Comercio', 2000000, 1, 345, '0', '0', 1, '', 1),
(37, 'Los Quebrachos', 'Domestico', 'Casa', 90000000, 5, 1000, '0', '0', 8, '', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `IdInquilino` int(11) NOT NULL,
  `Nombre` varchar(255) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Apellido` varchar(255) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Dni` varchar(10) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Telefono` varchar(15) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Email` varchar(100) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Activo` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish2_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`IdInquilino`, `Nombre`, `Apellido`, `Dni`, `Telefono`, `Email`, `Activo`) VALUES
(3, 'El', 'Peluca', '26963874', '12456987', 'elpeluca@gmail.com', 1),
(4, 'Peter', 'Parker', '2345612366', '2664852741', 'peter@gmail.com', 1),
(5, 'Pepe', 'Honguito', '89541257', '36985214', 'pepe@gmail.com', 0),
(6, 'Roberto', 'Sancho', '22456951', '96325874', 'sandro@hotmail.com', 1),
(7, 'pruebaDos', 'Dos', '25874159', '2665897412', 'dos@gmail.com', 0),
(8, 'El otro', 'Yose', '78963215', '123456', 'yo@hotmail.com', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `Id` int(11) NOT NULL,
  `NumeroPago` int(11) NOT NULL,
  `FechaPago` datetime NOT NULL,
  `Importe` decimal(10,0) NOT NULL,
  `Detalle` varchar(255) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `ContratoId` int(11) NOT NULL,
  `Anulado` tinyint(1) NOT NULL,
  `UsuarioAltaId` int(11) DEFAULT NULL,
  `UsuarioBajaId` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish2_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`Id`, `NumeroPago`, `FechaPago`, `Importe`, `Detalle`, `ContratoId`, `Anulado`, `UsuarioAltaId`, `UsuarioBajaId`) VALUES
(1, 1, '2025-04-07 09:00:00', '500000', 'Abril', 2, 1, NULL, NULL),
(2, 2, '2025-05-08 00:00:00', '500000', 'Mayo', 2, 1, NULL, NULL),
(3, 1, '2025-04-10 00:00:00', '500', 'Abril 2', 12, 0, NULL, NULL),
(4, 1, '2025-04-11 00:00:00', '6', 'Abril', 14, 0, NULL, NULL),
(9, 1, '2025-04-18 00:00:00', '1', 'Abril', 16, 0, 3, NULL),
(14, 2, '2025-04-18 00:00:00', '2', 'Multa por finalización anticipada', 16, 1, NULL, 3),
(15, 1, '2025-04-18 00:00:00', '2', 'Abril', 17, 0, 3, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `IdPropietario` int(11) NOT NULL,
  `Nombre` varchar(100) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Apellido` varchar(100) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Dni` varchar(100) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Telefono` varchar(20) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Email` varchar(100) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Clave` varchar(255) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Activo` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish2_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`IdPropietario`, `Nombre`, `Apellido`, `Dni`, `Telefono`, `Email`, `Clave`, `Activo`) VALUES
(1, 'Pep', 'Guardiola', '22589632', '44852963', 'pep@gmail.com', 'soypep', 1),
(2, 'Brad', 'Pitt', '123456', '25896314', 'brad@gmail.com', '5uyGOQPyQQ+fue9DYI4ulDe2dz4b4vaBZiV6ZQAZj7M=', 1),
(4, 'nn', 'Vaya a saber', '35789654', '2664508961', 'nn@yahoo.com.ar', 'UWjWQ0TIMwQT9dmHpxV/1KGV1TNuS3HNDzsPH+vv+38=', 1),
(5, 'Prueba', 'Prueba1', '23456123', '2665432145', 'prueba@gmail.com', '7dRxoaYiyFvLmnJRkvPVj0xfCf0WwbIO0eE1eLJbK9Q=', 1),
(6, 'Prueba2', 'Prueba2', '26852147', '25896374', 'prueba2@gmail.com', 'YOAadrIprHuZLaQR5etwlM8SifPCYXUK4O85hLcp+48=', 1),
(8, 'Damien', 'Yeah', '26789541', '2665874123', 'yeah@gmail.com', 'zLduHOWvIN18IbebBmAVIOwtz0vfT7OscNjH2eE0BJI=', 1),
(9, 'Otro', 'Nose o si', '78965412', '96325874', 'otro@hotmail.com', 'yChq/geXkHFjWSrMHv7bSYk+eLuRgyyQggXOcZRyc8s=', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(255) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Apellido` varchar(255) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Email` varchar(100) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Clave` varchar(255) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Avatar` varchar(255) COLLATE utf8mb4_spanish2_ci DEFAULT NULL,
  `Rol` int(11) NOT NULL,
  `Activo` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish2_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`Id`, `Nombre`, `Apellido`, `Email`, `Clave`, `Avatar`, `Rol`, `Activo`) VALUES
(3, 'Prueba', 'Uno', 'pruebauno@gmail.com', 'g3CQVP7XfU3mz0+5b7iwfhdXm+xmutqsCC/sPUnqqKA=', '/Uploads\\avatar_3.jpg', 2, 1),
(4, 'Damian', 'Nose', 'damian@gmail.com', 'GAKKw6Co5EiIGNiZC1OfQC6offL+e8CoEs3SX0LIrHA=', '/Uploads\\avatar_4.jpg', 1, 1),
(5, 'Otro', 'Prueba2', 'prueba2@gmail.com', 'M4MSS0mY3axhe8Si/cKmqKy8teVa30bnn7/RUMl3RXA=', '/Uploads\\avatar_5.jpg', 2, 1),
(6, 'Yeah', 'Sapee', 'yeah@gmail.com', 'm5gUL8CXv8nojwm8H0E+w3EuW+/kFwIiRs2d1hDW7Bk=', '/Uploads\\avatar_6.png', 1, 1),
(9, 'Pepe', 'Honguito', 'pepe@gmail.com', '6TMz0hl7AVtqb8dX74va59JyK/0J70baHXhQ443+WHI=', '/Uploads\\avatar_9.jpg', 2, 1);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdInquilino` (`IdInquilino`),
  ADD KEY `IdInmueble` (`IdInmueble`),
  ADD KEY `FK_Contratos_UsuarioAlta` (`UsuarioAltaId`),
  ADD KEY `FK_Contratos_UsuarioBaja` (`UsuarioBajaId`);

--
-- Indices de la tabla `imagenes`
--
ALTER TABLE `imagenes`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InmuebleId` (`InmuebleId`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `PropietarioId` (`PropietarioId`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`IdInquilino`),
  ADD UNIQUE KEY `Dni` (`Dni`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `ContratoId` (`ContratoId`),
  ADD KEY `FK_Pagos_UsuarioAlta` (`UsuarioAltaId`),
  ADD KEY `FK_Pagos_UsuarioBaja` (`UsuarioBajaId`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`IdPropietario`),
  ADD UNIQUE KEY `Dni` (`Dni`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT de la tabla `imagenes`
--
ALTER TABLE `imagenes`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=54;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=38;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `IdInquilino` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `IdPropietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `FK_Contratos_UsuarioAlta` FOREIGN KEY (`UsuarioAltaId`) REFERENCES `usuarios` (`Id`),
  ADD CONSTRAINT `FK_Contratos_UsuarioBaja` FOREIGN KEY (`UsuarioBajaId`) REFERENCES `usuarios` (`Id`),
  ADD CONSTRAINT `contratos_ibfk_1` FOREIGN KEY (`IdInquilino`) REFERENCES `inquilinos` (`IdInquilino`),
  ADD CONSTRAINT `contratos_ibfk_2` FOREIGN KEY (`IdInmueble`) REFERENCES `inmuebles` (`Id`);

--
-- Filtros para la tabla `imagenes`
--
ALTER TABLE `imagenes`
  ADD CONSTRAINT `imagenes_ibfk_1` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`Id`);

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `inmuebles_ibfk_1` FOREIGN KEY (`PropietarioId`) REFERENCES `propietarios` (`IdPropietario`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `FK_Pagos_UsuarioAlta` FOREIGN KEY (`UsuarioAltaId`) REFERENCES `usuarios` (`Id`),
  ADD CONSTRAINT `FK_Pagos_UsuarioBaja` FOREIGN KEY (`UsuarioBajaId`) REFERENCES `usuarios` (`Id`),
  ADD CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`ContratoId`) REFERENCES `contratos` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
