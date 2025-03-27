-- phpMyAdmin SQL Dump
-- version 4.8.5
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 27-03-2025 a las 20:33:53
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
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `IdInquilino` int(11) NOT NULL,
  `Nombre` varchar(255) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Apellido` varchar(255) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Dni` varchar(10) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Telefono` varchar(15) COLLATE utf8mb4_spanish2_ci NOT NULL,
  `Email` varchar(100) COLLATE utf8mb4_spanish2_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish2_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`IdInquilino`, `Nombre`, `Apellido`, `Dni`, `Telefono`, `Email`) VALUES
(3, 'El', 'Peluca', '26963874', '12456987', 'elpeluca@gmail.com'),
(4, 'Peter', 'Parker', '2345612366', '2664852741', 'peter@gmail.com');

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
  `Clave` varchar(255) COLLATE utf8mb4_spanish2_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_spanish2_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`IdPropietario`, `Nombre`, `Apellido`, `Dni`, `Telefono`, `Email`, `Clave`) VALUES
(1, 'Pep', 'Guardiola', '22589632', '44852963', 'pep@gmail.com', 'soypep'),
(2, 'Brad', 'Pitt', '123456', '25896314', 'brad@gmail.com', '5uyGOQPyQQ+fue9DYI4ulDe2dz4b4vaBZiV6ZQAZj7M='),
(4, 'nn', 'Vaya a saber', '35789654', '2664508961', 'nn@yahoo.com.ar', 'UWjWQ0TIMwQT9dmHpxV/1KGV1TNuS3HNDzsPH+vv+38='),
(5, 'Prueba', 'Prueba1', '23456123', '2665432145', 'prueba@gmail.com', '7dRxoaYiyFvLmnJRkvPVj0xfCf0WwbIO0eE1eLJbK9Q=');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`IdInquilino`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`IdPropietario`),
  ADD UNIQUE KEY `Dni` (`Dni`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `IdInquilino` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `IdPropietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
