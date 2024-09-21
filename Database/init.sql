--
-- PostgreSQL database dump
--

-- Dumped from database version 15.4
-- Dumped by pg_dump version 15.3

-- Started on 2024-09-01 16:24:12

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 3365 (class 1262 OID 16577)
-- Name: recipes; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE "recipes" WITH TEMPLATE = template0 ENCODING = 'UTF8';


ALTER DATABASE "recipes" OWNER TO postgres;

\connect "recipes"

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 218 (class 1259 OID 16599)
-- Name: ingredient; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ingredient (
    id integer NOT NULL,
    ownerid integer NOT NULL,
    name character varying(200) NOT NULL,
    created timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    deleted timestamp with time zone
);


ALTER TABLE public.ingredient OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16598)
-- Name: ingredient_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.ingredient ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.ingredient_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 216 (class 1259 OID 16586)
-- Name: owner; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.owner (
    id integer NOT NULL,
    email character varying(50),
    code character varying(50),
    created timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    code_updated timestamp with time zone,
    deactivated timestamp with time zone
);


ALTER TABLE public.owner OWNER TO postgres;

--
-- TOC entry 214 (class 1259 OID 16578)
-- Name: recipe; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.recipe (
    id integer NOT NULL,
    owner_id integer NOT NULL,
    name character varying(100) NOT NULL,
    description character varying(250),
    content character varying(2000) NOT NULL,
    duration integer,
    created timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    deleted timestamp with time zone
);


ALTER TABLE public.recipe OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 16625)
-- Name: recipe_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.recipe ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.recipe_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 219 (class 1259 OID 16610)
-- Name: recipeingredientlink; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.recipeingredientlink (
    recipeid integer NOT NULL,
    ingredientid integer NOT NULL,
    amount numeric(10,3),
    unit character varying(100)
);


ALTER TABLE public.recipeingredientlink OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 16637)
-- Name: recipetaglink; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.recipetaglink (
    tagid integer NOT NULL,
    recipeid integer NOT NULL
);


ALTER TABLE public.recipetaglink OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 16627)
-- Name: tag; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tag (
    id integer NOT NULL,
    ownerid integer NOT NULL,
    name character varying(50)
);


ALTER TABLE public.tag OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16626)
-- Name: tag_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.tag ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.tag_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 215 (class 1259 OID 16585)
-- Name: user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.owner ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 3200 (class 2606 OID 16584)
-- Name: recipe Recipe_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipe
    ADD CONSTRAINT "Recipe_pkey" PRIMARY KEY (id);


--
-- TOC entry 3204 (class 2606 OID 16604)
-- Name: ingredient ingredient_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient
    ADD CONSTRAINT ingredient_pkey PRIMARY KEY (id);


--
-- TOC entry 3206 (class 2606 OID 16614)
-- Name: recipeingredientlink recipeingredientlink_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipeingredientlink
    ADD CONSTRAINT recipeingredientlink_pkey PRIMARY KEY (recipeid, ingredientid);


--
-- TOC entry 3210 (class 2606 OID 16641)
-- Name: recipetaglink recipetaglink_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipetaglink
    ADD CONSTRAINT recipetaglink_pkey PRIMARY KEY (tagid, recipeid);


--
-- TOC entry 3208 (class 2606 OID 16631)
-- Name: tag tag_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tag
    ADD CONSTRAINT tag_pkey PRIMARY KEY (id);


--
-- TOC entry 3202 (class 2606 OID 16590)
-- Name: owner user_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.owner
    ADD CONSTRAINT user_pkey PRIMARY KEY (id);


--
-- TOC entry 3212 (class 2606 OID 16605)
-- Name: ingredient ingredient_owner_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient
    ADD CONSTRAINT ingredient_owner_fkey FOREIGN KEY (ownerid) REFERENCES public.owner(id) NOT VALID;


--
-- TOC entry 3213 (class 2606 OID 16620)
-- Name: recipeingredientlink link_ingredient_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipeingredientlink
    ADD CONSTRAINT link_ingredient_fkey FOREIGN KEY (ingredientid) REFERENCES public.ingredient(id) NOT VALID;


--
-- TOC entry 3214 (class 2606 OID 16615)
-- Name: recipeingredientlink link_recipe_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipeingredientlink
    ADD CONSTRAINT link_recipe_fkey FOREIGN KEY (recipeid) REFERENCES public.recipe(id) NOT VALID;


--
-- TOC entry 3216 (class 2606 OID 16647)
-- Name: recipetaglink link_recipe_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipetaglink
    ADD CONSTRAINT link_recipe_fkey FOREIGN KEY (recipeid) REFERENCES public.recipe(id);


--
-- TOC entry 3217 (class 2606 OID 16642)
-- Name: recipetaglink link_tag_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipetaglink
    ADD CONSTRAINT link_tag_fkey FOREIGN KEY (tagid) REFERENCES public.tag(id);


--
-- TOC entry 3211 (class 2606 OID 16591)
-- Name: recipe recipe_owner_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipe
    ADD CONSTRAINT recipe_owner_fkey FOREIGN KEY (owner_id) REFERENCES public.owner(id) NOT VALID;


--
-- TOC entry 3215 (class 2606 OID 16632)
-- Name: tag tag_owner_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tag
    ADD CONSTRAINT tag_owner_fkey FOREIGN KEY (ownerid) REFERENCES public.owner(id);

